using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthMultiplayerGame.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthMultiplayerGame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JWTSettings _options;

    public AuthorizeController(UserManager<IdentityUser> user, 
        SignInManager<IdentityUser> signIn, IOptions<JWTSettings> options)
    {
        _userManager = user;
        _signInManager = signIn;
        _options = options.Value;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(ParamRegister paramRegister)
    {
        // Create a new IdentityUser based on registration parameters
        var user = new IdentityUser{ UserName = paramRegister.Username, Email = paramRegister.Email };
        // Attempt to create a new user
        var result = await _userManager.CreateAsync(user, paramRegister.Password!);

        // Check if user creation was successful
        if (!result.Succeeded) { return BadRequest(); }

        // Sign in the newly registered user (not persistent)
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        // Add custom claims to the user if needed
        // var claims = new List<Claim>{ new Claim() };
        // await _userManager.AddClaimsAsync(user, claims);

        // Return an Ok response indicating successful registration
        return Ok();
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(ParamLogin paramLogin)
    {
        //var passwordHash = _hasher.HashString(paramLogin.Password!);
        // Find the user by email
        var user = await _userManager.FindByEmailAsync(paramLogin.Email!);
        // Attempt to sign in the user with the provided password
        var result = await _signInManager.PasswordSignInAsync(
            user!, 
            paramLogin.Password!,  
            false, 
            false
        );

        // Check if sign-in was successful
        if (!result.Succeeded) { return BadRequest(); }

        // Get claims associated with the user
        IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user!);
        // Generate a JWT token using user information and claims
        var token = GetToken(user!, claims);
        
        // Return the generated token as an Ok response
        return Ok(token);
    }

    // Method to generate a JWT token
    private string GetToken(IdentityUser user, IEnumerable<Claim> principal)
    {
        // Convert claims to a list for manipulation
        var claims = principal.ToList();
        // Add a Name claim with the user's username
        claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

        // Create a symmetric security key based on the configured secret key
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey!)
        );
        // Create a new JWT with specified parameters
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );

        // Write the JWT token as a string
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
