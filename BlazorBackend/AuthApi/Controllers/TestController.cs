using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApi.Controllers;

/// <summary>
/// Provides test endpoints for checking public and authenticated API access.
/// Used to verify JWT authentication behavior.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Public endpoint that can be accessed without authentication.
    /// </summary>
    /// <returns>
    /// A simple message confirming that the endpoint is publicly accessible.
    /// </returns>
    [HttpGet("public")]
    public IActionResult Public() =>
        Ok(new
        {
            // Message returned to unauthenticated clients.
            message = "This is a public endpoint."
        });

    /// <summary>
    /// Secure endpoint that requires a valid JWT authentication token.
    ///
    /// The user identity information is extracted from the claims
    /// contained inside the JWT token.
    /// </summary>
    /// <returns>
    /// Authentication confirmation along with user information.
    /// </returns>
    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok(new
        {
            // Confirmation message for authenticated users.
            message = "You are authenticated.",

            // Retrieves the user's unique identifier from JWT claims.
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),

            // Retrieves the user's email from JWT claims.
            // If email claim does not exist, uses the identity name instead.
            email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.Identity?.Name
        });
    }
}