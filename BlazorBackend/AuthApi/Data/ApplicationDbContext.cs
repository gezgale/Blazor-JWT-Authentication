using AuthApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data;

/// <summary>
/// Represents the Entity Framework Core database context for the application.
///
/// This context extends IdentityDbContext to provide database access for
/// ASP.NET Core Identity entities such as users, roles, claims, logins,
/// and user tokens, along with custom application user properties.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    ///
    /// The options parameter contains database configuration information,
    /// such as the database provider and connection string.
    /// </summary>
    /// <param name="options">
    /// Configuration options used by Entity Framework Core.
    /// </param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}