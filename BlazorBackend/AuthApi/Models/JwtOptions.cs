namespace AuthApi.Models;

/// <summary>
/// Represents the JWT (JSON Web Token) configuration settings.
/// Values are typically loaded from the "Jwt" section of appsettings.json.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Secret key used to sign and validate JWT tokens.
    /// This key should be kept secure and never exposed publicly.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Name of the application or service that issues the JWT.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// The intended recipient (client or application) that can use the token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Specifies how long the generated JWT remains valid,
    /// measured in minutes.
    /// </summary>
    public int ExpiresInMinutes { get; set; }
}