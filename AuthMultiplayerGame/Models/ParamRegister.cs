using System.ComponentModel.DataAnnotations;

namespace AuthMultiplayerGame.Models;

// Class representing user parameters for Register
public class ParamRegister : ParamLogin
{
    [Required(ErrorMessage = "Username is required.")]
    public string? Username { get; set; }
}
