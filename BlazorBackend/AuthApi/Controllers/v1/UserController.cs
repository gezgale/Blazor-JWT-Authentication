#region NameSpaces
using Asp.Versioning;
using AuthApi.DTOs;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace AuthApi.Controllers.v1;
#endregion

/// <summary>
/// کنترولر اصلی برای ورود به سیستم
/// </summary>
[ApiVersion("1")]
public class UserController : BaseController
{
    #region Variables
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwtTokenService;
    #endregion

    public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "A user with this email already exists."
            });
        }
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = string.Join(" | ", result.Errors.Select(e => e.Description))
            });
        }
        var (token, expiresAt) = await _jwtTokenService.CreateTokenAsync(user);
        return Ok(new AuthResponse
        {
            Success = true,
            Message = "User registered successfully.",
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

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "نام کاربری یا کمله عبور به درستی وارد نشده است."
            });
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "نام کاربری یا کمله عبور به درستی وارد نشده است."
            });
        }
        var (token, expiresAt) = await _jwtTokenService.CreateTokenAsync(user);
        return Ok(new AuthResponse
        {
            Success = true,
            Message = "ورود موفقیت آمیز.",
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
}