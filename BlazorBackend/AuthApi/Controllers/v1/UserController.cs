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
/// کنترلر اصلی مدیریت عملیات احراز هویت کاربران
/// شامل ثبت نام و ورود به سیستم می‌باشد.
/// </summary>
[ApiVersion("1")]
public class UserController : BaseController
{
    #region Variables

    // سرویس مدیریت کاربران Identity
    private readonly UserManager<ApplicationUser> _userManager;

    // سرویس مدیریت ورود کاربران Identity
    private readonly SignInManager<ApplicationUser> _signInManager;

    // سرویس ایجاد و مدیریت JWT Token
    private readonly JwtTokenService _jwtTokenService;

    #endregion

    /// <summary>
    /// سازنده کنترلر User
    /// وابستگی‌های مورد نیاز از طریق Dependency Injection دریافت می‌شوند.
    /// </summary>
    /// <param name="userManager">مدیریت کاربران Identity</param>
    /// <param name="signInManager">مدیریت عملیات ورود کاربران</param>
    /// <param name="jwtTokenService">سرویس تولید JWT Token</param>
    public UserController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// ثبت نام کاربر جدید در سیستم.
    /// پس از ثبت موفق، یک JWT Token برای کاربر ایجاد می‌شود.
    /// </summary>
    /// <param name="request">
    /// اطلاعات مورد نیاز برای ثبت نام شامل نام، ایمیل و رمز عبور
    /// </param>
    /// <returns>
    /// اطلاعات کاربر به همراه Token دسترسی
    /// </returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // بررسی اعتبار مدل ورودی
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // بررسی وجود کاربر با ایمیل مشابه
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "A user with this email already exists."
            });
        }

        // ایجاد نمونه کاربر جدید
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,

            // تایید ایمیل به صورت خودکار
            EmailConfirmed = true
        };

        // ایجاد کاربر در Identity
        var result = await _userManager.CreateAsync(user, request.Password);

        // بررسی خطاهای ثبت کاربر
        if (!result.Succeeded)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,

                // ترکیب خطاهای Identity در یک پیام
                Message = string.Join(
                    " | ",
                    result.Errors.Select(e => e.Description))
            });
        }

        // ایجاد JWT Token برای کاربر ثبت شده
        var (token, expiresAt) =
            await _jwtTokenService.CreateTokenAsync(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "User registered successfully.",

            // ارسال Token به کلاینت
            Token = token,

            // زمان انقضای Token
            ExpiresAt = expiresAt,

            // اطلاعات عمومی کاربر
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName
            }
        });
    }


    /// <summary>
    /// ورود کاربر به سیستم.
    /// اطلاعات ورود بررسی شده و در صورت موفقیت JWT Token ایجاد می‌شود.
    /// </summary>
    /// <param name="request">
    /// شامل ایمیل و رمز عبور کاربر
    /// </param>
    /// <returns>
    /// اطلاعات کاربر و Token دسترسی
    /// </returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // بررسی اعتبار داده‌های ورودی
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // پیدا کردن کاربر با ایمیل
        var user = await _userManager.FindByEmailAsync(request.Email);

        // اگر کاربر وجود نداشت
        if (user is null)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,

                // پیام عمومی برای جلوگیری از افشای اطلاعات امنیتی
                Message = "نام کاربری یا کلمه عبور به درستی وارد نشده است."
            });
        }

        // بررسی صحت رمز عبور
        var result = await _signInManager
            .CheckPasswordSignInAsync(
                user,
                request.Password,
                false);

        // رمز عبور اشتباه است
        if (!result.Succeeded)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "نام کاربری یا کلمه عبور به درستی وارد نشده است."
            });
        }

        // ایجاد JWT Token پس از ورود موفق
        var (token, expiresAt) =
            await _jwtTokenService.CreateTokenAsync(user);

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