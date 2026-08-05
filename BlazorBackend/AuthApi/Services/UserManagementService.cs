using AuthApi.DTOs;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Services;

/// <summary>
/// Service responsible for managing user-related operations.
/// </summary>
public class UserManagementService
{
    // ASP.NET Core Identity service used to manage application users.
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserManagementService"/> class.
    /// </summary>
    /// <param name="userManager">
    /// The UserManager service provided by ASP.NET Core Identity.
    /// </param>
    public UserManagementService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves all users from the database and maps them to
    /// <see cref="UserManagementDto"/> objects.
    /// </summary>
    /// <returns>
    /// A collection of users containing only the information
    /// required by the client.
    /// </returns>
    public IEnumerable<UserManagementDto> GetUsers() =>
        _userManager.Users.Select(x => new UserManagementDto
        {
            // Unique identifier of the user.
            Id = x.Id,

            // User's full name.
            FullName = x.FullName,

            // User's email address.
            Email = x.Email,

            // Username used for login.
            UserName = x.UserName,

            // Indicates whether the user's email has been confirmed.
            EmailConfirmed = x.EmailConfirmed
        });
}