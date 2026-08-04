using AuthApi.Framework.SharedKernel.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[ApiResultFilter]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[ApiExplorerSettings(IgnoreApi = false)]
public class BaseController : ControllerBase
{
    /// <summary>
    /// Logger instance for all derived controllers
    /// </summary>
    protected ILogger<BaseController>? Logger { get; set; }

    /// <summary>
    /// </summary>
    public bool UserIsAuthenticated => HttpContext.User.Identity!.IsAuthenticated;

    /// <summary>
    /// Get current user ID from claims
    /// </summary>
    protected string? GetCurrentUserId() =>
        HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Get current user name from claims
    /// </summary>
    protected string? GetCurrentUserName() =>
        HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

    /// <summary>
    /// Safe logging method to prevent null reference exceptions
    /// </summary>
    protected void LogInformation(string message, params object?[] args)
    {
        Logger?.LogInformation(message, args);
    }

    /// <summary>
    /// Safe logging method for warnings
    /// </summary>
    protected void LogWarning(string message, params object?[] args)
    {
        Logger?.LogWarning(message, args);
    }

    /// <summary>
    /// Safe logging method for errors
    /// </summary>
    protected void LogError(Exception? exception, string message, params object?[] args)
    {
        Logger?.LogError(exception, message, args);
    }
}