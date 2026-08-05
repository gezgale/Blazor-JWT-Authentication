using AuthApi.DTOs;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApi.Controllers;

/// <summary>
/// Handles authentication-related operations such as user registration,
/// login, and retrieving the current authenticated user's information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // ASP.NET Core Identity service for managing users.
    private readonly UserManager<ApplicationUser> _userManager;

    // ASP.NET Core Identity service for handling sign-in operations.
    private readonly SignInManager<ApplicationUser> _signInManager;

    // Service responsible for creating JWT authentication tokens.
    private readonly JwtTokenService _jwtTokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">User management service.</param>
    /// <param name="signInManager">Sign-in management service.</param>
    /// <param name="jwtTokenService">JWT token generation service.</param>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Registers a new user account.
    /// Creates the user in ASP.NET Core Identity and returns a JWT token.
    /// </summary>
    /// <param name="request">Registration information.</param>
    /// <returns>
    /// Authentication response containing user details and JWT token.
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Validate incoming request model.
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if a user with the same email already exists.
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "A user with this email already exists."
            });
        }

        // Create a new application user entity.
        var user = new ApplicationUser
        {
            // Use email as username.
            UserName = request.Email,

            // Store user email.
            Email = request.Email,

            // Store user's full name.
            FullName = request.FullName,

            // Email confirmation is enabled automatically.
            EmailConfirmed = true
        };

        // Create user with the provided password.
        var result = await _userManager.CreateAsync(user, request.Password);

        // Return identity validation errors if creation fails.
        if (!result.Succeeded)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = string.Join(
                    " | ",
                    result.Errors.Select(e => e.Description)
                )
            });
        }

        // Generate JWT token for the newly registered user.
        var (token, expiresAt) =
            await _jwtTokenService.CreateTokenAsync(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "User registered successfully.",
            Token = token,
            ExpiresAt = expiresAt,

            // Return basic user information.
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName
            }
        });
    }

    /// <summary>
    /// Authenticates an existing user.
    /// Validates credentials and returns a JWT token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>
    /// Authentication response containing JWT token and user information.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // Validate incoming request model.
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Find user by email.
        var user = await _userManager.FindByEmailAsync(request.Email);

        // User does not exist.
        if (user is null)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password."
            });
        }

        // Validate user's password.
        var result = await _signInManager
            .CheckPasswordSignInAsync(user, request.Password, false);

        // Password is incorrect.
        if (!result.Succeeded)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password."
            });
        }

        // Generate JWT token after successful authentication.
        var (token, expiresAt) =
            await _jwtTokenService.CreateTokenAsync(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful.",
            Token = token,
            ExpiresAt = expiresAt,

            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName
            }
        });
    }

    /// <summary>
    /// Returns information about the currently authenticated user.
    /// Requires a valid JWT token.
    /// </summary>
    /// <returns>
    /// The current user's profile information.
    /// </returns>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // Extract user ID from JWT claims.
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // User ID was not found in token.
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        // Retrieve user from database.
        var user = await _userManager.FindByIdAsync(userId);

        // User does not exist.
        if (user is null)
            return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName
        });
    }

    /// <summary>
    /// Test endpoint.
    /// Returns a simple text response.
    /// </summary>
    /// <returns>A test message.</returns>
    [HttpGet("zaz")]
    public async Task<IActionResult> Zaz()
    {
        return Ok("شدنمتسنم یتنمشستن یشستنم");
    }
}