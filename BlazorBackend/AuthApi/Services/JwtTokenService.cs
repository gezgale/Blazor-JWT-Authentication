using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

/// <summary>
/// Service responsible for creating JSON Web Tokens (JWT)
/// for authenticated users.
/// </summary>
public class JwtTokenService
{
    // Provides access to application configuration (appsettings.json).
    private readonly IConfiguration _configuration;

    // ASP.NET Core Identity service used to retrieve user roles.
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration containing JWT settings.
    /// </param>
    /// <param name="userManager">
    /// ASP.NET Core Identity UserManager used for retrieving user information.
    /// </param>
    public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    /// <summary>
    /// Creates a signed JWT token for the specified user.
    /// </summary>
    /// <param name="user">
    /// The authenticated user for whom the token will be generated.
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// - token: The generated JWT as a string.
    /// - expiresAt: The UTC expiration date and time.
    /// </returns>
    public async Task<(string token, DateTime expiresAt)> CreateTokenAsync(ApplicationUser user)
    {
        // Read JWT configuration values.
        var jwtSection = _configuration.GetSection("Jwt");

        // Secret key used to sign the token.
        var key = jwtSection["Key"]!;

        // Token issuer.
        var issuer = jwtSection["Issuer"]!;

        // Intended audience.
        var audience = jwtSection["Audience"]!;

        // Token lifetime in minutes.
        var expiresInMinutes = int.Parse(jwtSection["ExpiresInMinutes"] ?? "1");

        // Retrieve all roles assigned to the user.
        var roles = await _userManager.GetRolesAsync(user);

        // Create the list of claims that will be embedded in the token.
        var claims = new List<Claim>
        {
            // Unique user identifier (JWT standard claim).
            new(JwtRegisteredClaimNames.Sub, user.Id),

            // User's email address.
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

            // ASP.NET Identity user identifier.
            new(ClaimTypes.NameIdentifier, user.Id),

            // Username displayed by the application.
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),

            // Custom claim containing the user's full name.
            new("fullName", user.FullName ?? string.Empty),

            // Unique identifier for this token.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add each user role as a Role claim.
        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role))
        );

        // Create the symmetric signing key.
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        // Create signing credentials using HMAC SHA-256.
        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256
        );

        // Calculate the token expiration time.
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        // Build the JWT.
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        // Return the serialized JWT string along with its expiration time.
        return (
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt
        );
    }
}