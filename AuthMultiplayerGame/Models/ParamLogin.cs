using System.ComponentModel.DataAnnotations;

namespace AuthMultiplayerGame.Models;

// Class representing user parameters for Login
public class ParamLogin
{
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
