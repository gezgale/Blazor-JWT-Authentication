using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs;

/// <summary>
/// Represents the data required when a user attempts to log in.
/// Contains the user's email address and password.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User's email address used for authentication.
    /// The value is required and must be in a valid email format.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password used for authentication.
    /// The value is required.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}