namespace AuthApi.DTOs;

/// <summary>
/// Represents user information returned to the client.
/// This DTO is used to expose only the required user data
/// instead of returning the complete Identity user entity.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user.
    /// This value is optional.
    /// </summary>
    public string? FullName { get; set; }
}