using System.Text;
using AuthMultiplayerGame;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
// Configure the DbContext to use SQLite with the connection string from configuration
builder.Services.AddDbContext<UserDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("UsersDB"))
);
// Add Identity services with Entity Framework storage for users and roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<UserDbContext>();
// Configure JWTSettings options from configuration
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

// Retrieve JWT settings from configuration
var issuer = builder.Configuration.GetSection("JWTSettings:Issuer").Value;
var audience = builder.Configuration.GetSection("JWTSettings:Audience").Value;
var secretKey = builder.Configuration.GetSection("JWTSettings:SecretKey").Value;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

// Configure authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        IssuerSigningKey = signingKey,
        ValidateIssuerSigningKey = true
    };
});
// Add controllers, API explorer, and Swagger documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();
// Map controllers
app.MapControllers();

app.Run();
