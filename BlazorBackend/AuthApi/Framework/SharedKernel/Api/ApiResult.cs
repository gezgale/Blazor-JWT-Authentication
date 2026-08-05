using AuthApi.Framework.Common.Enums;
using AuthApi.Framework.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthApi.Framework.SharedKernel.Api;

/// <summary>
/// Represents a standard API response without a data payload.
/// All API responses are wrapped in this class to provide
/// a consistent response format.
/// </summary>
public class ApiResult
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Application-specific status code.
    /// </summary>
    public ApiResultStatusCode StatusCode { get; set; }

    /// <summary>
    /// Optional response message.
    /// Will not be serialized if its value is null.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ApiResult"/>.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="statusCode">Application status code.</param>
    /// <param name="message">
    /// Optional response message.
    /// If omitted, the display name of the status code is used.
    /// </param>
    public ApiResult(bool isSuccess, ApiResultStatusCode statusCode, string message = null!)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Message = message ?? statusCode.ToDisplay();
    }

    #region Implicit Operators

    /// <summary>
    /// Converts an ASP.NET Core OkResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult(OkResult result)
    {
        return new ApiResult(true, ApiResultStatusCode.Success);
    }

    /// <summary>
    /// Converts a BadRequestResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult(BadRequestResult result)
    {
        return new ApiResult(false, ApiResultStatusCode.BadRequest);
    }

    /// <summary>
    /// Converts a BadRequestObjectResult into ApiResult.
    /// Validation errors are extracted and combined into a single message.
    /// </summary>
    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();

        // Handle model validation errors.
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors
                .SelectMany(p => (string[])p.Value)
                .Distinct();

            message = string.Join(" | ", errorMessages);
        }

        return new ApiResult(false, ApiResultStatusCode.BadRequest, message!);
    }

    /// <summary>
    /// Converts a ContentResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult(ContentResult result)
    {
        return new ApiResult(true, ApiResultStatusCode.Success, result.Content!);
    }

    /// <summary>
    /// Converts a NotFoundResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult(NotFoundResult result)
    {
        return new ApiResult(false, ApiResultStatusCode.NotFound);
    }

    #endregion
}

/// <summary>
/// Represents a standard API response that contains data.
/// </summary>
/// <typeparam name="TData">
/// Type of the response payload.
/// </typeparam>
public class ApiResult<TData> : ApiResult
       where TData : class
{
    /// <summary>
    /// Response payload.
    /// Will not be serialized if null.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TData Data { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ApiResult{TData}"/>.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="statusCode">Application status code.</param>
    /// <param name="data">Response data.</param>
    /// <param name="message">Optional response message.</param>
    public ApiResult(
        bool isSuccess,
        ApiResultStatusCode statusCode,
        TData data,
        string message = null!)
        : base(isSuccess, statusCode, message)
    {
        Data = data;
    }

    #region Implicit Operators

    /// <summary>
    /// Converts an object directly into a successful ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(TData data)
    {
        return new ApiResult<TData>(
            true,
            ApiResultStatusCode.Success,
            data);
    }

    /// <summary>
    /// Converts OkResult into ApiResult with no payload.
    /// </summary>
    public static implicit operator ApiResult<TData>(OkResult result)
    {
        return new ApiResult<TData>(
            true,
            ApiResultStatusCode.Success,
            null!);
    }

    /// <summary>
    /// Converts OkObjectResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(OkObjectResult result)
    {
        return new ApiResult<TData>(
            true,
            ApiResultStatusCode.Success,
            ((TData)result.Value!)!);
    }

    /// <summary>
    /// Converts BadRequestResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(BadRequestResult result)
    {
        return new ApiResult<TData>(
            false,
            ApiResultStatusCode.BadRequest,
            null!);
    }

    /// <summary>
    /// Converts BadRequestObjectResult into ApiResult.
    /// Extracts validation errors when available.
    /// </summary>
    public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();

        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors
                .SelectMany(p => (string[])p.Value)
                .Distinct();

            message = string.Join(" | ", errorMessages);
        }

        return new ApiResult<TData>(
            false,
            ApiResultStatusCode.BadRequest,
            null!,
            message!);
    }

    /// <summary>
    /// Converts ContentResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(ContentResult result)
    {
        return new ApiResult<TData>(
            true,
            ApiResultStatusCode.Success,
            null!,
            result.Content!);
    }

    /// <summary>
    /// Converts NotFoundResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(NotFoundResult result)
    {
        return new ApiResult<TData>(
            false,
            ApiResultStatusCode.NotFound,
            null!);
    }

    /// <summary>
    /// Converts NotFoundObjectResult into ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
    {
        return new ApiResult<TData>(
            false,
            ApiResultStatusCode.NotFound,
            ((TData)result.Value!)!);
    }

    #endregion
}