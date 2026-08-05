using Asp.Versioning;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.v1;

/// <summary>
/// کنترلر مدیریت کاربران سیستم.
/// شامل عملیات مربوط به مشاهده و مدیریت اطلاعات کاربران می‌باشد.
/// </summary>
[ApiVersion("1")]
public class UserManagementController : BaseController
{
    // سرویس مدیریت کاربران که منطق مربوط به کاربران را انجام می‌دهد.
    private readonly UserManagementService _service;

    /// <summary>
    /// سازنده کنترلر مدیریت کاربران.
    /// سرویس مورد نیاز از طریق Dependency Injection دریافت می‌شود.
    /// </summary>
    /// <param name="service">
    /// سرویس مدیریت کاربران
    /// </param>
    public UserManagementController(UserManagementService service)
    {
        _service = service;
    }

    /// <summary>
    /// دریافت لیست تمام کاربران سیستم.
    /// </summary>
    /// <returns>
    /// مجموعه‌ای از کاربران موجود در سیستم.
    /// </returns>
    [HttpGet("GetUsers")]
    public IActionResult GetUsers()
    {
        // دریافت کاربران از سرویس و ارسال به کلاینت
        return Ok(_service.GetUsers());
    }


    /// <summary>
    /// بروزرسانی اطلاعات کاربر.
    /// 
    /// در حال حاضر فقط اطلاعات دریافتی را برمی‌گرداند.
    /// منطق بروزرسانی دیتابیس باید در سرویس اضافه شود.
    /// </summary>
    /// <param name="dto">
    /// اطلاعات کاربر جهت بروزرسانی.
    /// </param>
    /// <returns>
    /// اطلاعات دریافت شده برای تست API.
    /// </returns>
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser(
        [FromBody] AuthApi.DTOs.UserManagementDto dto)
    {
        // TODO:
        // در این بخش باید عملیات Update کاربر با استفاده از UserManager
        // یا UserManagementService انجام شود.

        return Ok(dto);
    }
}