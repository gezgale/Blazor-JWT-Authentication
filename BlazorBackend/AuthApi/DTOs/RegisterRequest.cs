using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs;

/// <summary>
/// Represents the data required when creating a new user account.
/// Contains the user's personal information and authentication credentials.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// The full name of the user.
    /// This field is required during registration.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address.
    /// This field is required and must be in a valid email format.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's password.
    /// This field is required and must contain at least 6 characters.
    /// </summary>
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}