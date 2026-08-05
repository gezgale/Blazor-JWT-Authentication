namespace AuthApi.DTOs;

/// <summary>
/// Represents user information used for user management operations.
/// This DTO contains administrative user details returned from the system.
/// </summary>
public class UserManagementDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Full name of the user.
    /// This value is optional.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Email address of the user.
    /// This value is optional.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Username used by the user to log in.
    /// This value is optional.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool EmailConfirmed { get; set; }
}