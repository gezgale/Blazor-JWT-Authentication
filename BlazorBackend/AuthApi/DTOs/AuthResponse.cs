namespace AuthApi.DTOs;

/// <summary>
/// Represents the response returned after an authentication operation.
/// Contains the authentication result, JWT token information,
/// and authenticated user details.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Indicates whether the authentication operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Provides a message describing the result of the authentication request.
    /// This can contain success information or an error description.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// JWT access token generated after successful authentication.
    /// This token is used for accessing protected API endpoints.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// The date and time when the JWT token expires.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Information about the authenticated user.
    /// </summary>
    public UserDto? User { get; set; }
}