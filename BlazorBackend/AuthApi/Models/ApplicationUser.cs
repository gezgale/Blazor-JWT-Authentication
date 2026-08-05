using Microsoft.AspNetCore.Identity;

namespace AuthApi.Models;

/// <summary>
/// Represents an application user.
/// Inherits from ASP.NET Core IdentityUser and adds
/// custom properties specific to the application.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the user's full name.
    /// This is an optional custom property that extends
    /// the default IdentityUser model.
    /// </summary>
    public string? FullName { get; set; }
}