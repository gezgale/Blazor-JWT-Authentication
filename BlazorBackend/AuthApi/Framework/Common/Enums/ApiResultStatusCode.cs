using System.ComponentModel.DataAnnotations;

namespace AuthApi.Framework.Common.Enums;

/// <summary>
/// Represents application-specific API response status codes.
/// Each value is associated with a user-friendly display message
/// using the <see cref="DisplayAttribute"/>.
/// </summary>
public enum ApiResultStatusCode
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    [Display(Name = "عملیات با موفقیت انجام شد")]
    Success = 0,

    /// <summary>
    /// An unexpected server error occurred.
    /// </summary>
    [Display(Name = "خطایی در سرور رخ داده است")]
    ServerError = 1,

    /// <summary>
    /// The request contains invalid or missing parameters.
    /// </summary>
    [Display(Name = "پارامتر های ارسالی معتبر نیستند")]
    BadRequest = 2,

    /// <summary>
    /// The requested resource could not be found.
    /// </summary>
    [Display(Name = "یافت نشد")]
    NotFound = 3,

    /// <summary>
    /// The requested collection contains no items.
    /// </summary>
    [Display(Name = "لیست خالی است")]
    ListEmpty = 4,

    /// <summary>
    /// A business logic or processing error occurred.
    /// </summary>
    [Display(Name = "خطایی در پردازش رخ داد")]
    LogicError = 5,

    /// <summary>
    /// Authentication or authorization failed.
    /// </summary>
    [Display(Name = "خطای احراز هویت")]
    UnAuthorized = 6,

    /// <summary>
    /// The server is unavailable or cannot be reached.
    /// </summary>
    [Display(Name = "سرور در دسترس نیست")]
    ServerUnreachable = 7
}