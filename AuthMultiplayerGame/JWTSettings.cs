namespace AuthMultiplayerGame;

// Class representing settings for JSON Web Token (JWT) configuration
public class JWTSettings
{
    // Secret key used for JWT signing and validation
    public string? SecretKey { get; set; }

    // Issuer of the JWT (entity that issues the token)
    public string? Issuer { get; set; }

    // Audience of the JWT (intended recipient of the token)
    public string? Audience { get; set; }
}
