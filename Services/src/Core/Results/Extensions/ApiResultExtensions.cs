using System.Net;

using KubeFood.Core.Results.Api;
using KubeFood.Core.Results.Base;
using KubeFood.Core.Results.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KubeFood.Core.Results.Extensions;

public static class ApiResultExtensions
{
    public static ApiResult ToApiResult(this IResultBase result, HttpStatusCode statusCode = HttpStatusCode.NoContent)
    {
        return new ApiResult(result, statusCode);
    }

    public static ApiResult ToApiResult<TValue>(this IResultBase<TValue> result, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        statusCode = result.Value is null
            ? HttpStatusCode.NoContent
            : statusCode;

        return new ApiResult<TValue>(result, statusCode);
    }

    public static ObjectResult ToActionResult(this IApiResult apiResult)
    {
        if (apiResult is ApiResult apiResultBase && !apiResultBase.Success)
        {
            var error = apiResultBase.Errors[0];
            apiResultBase.SetStatus(error.Status);
        }

        var result =
            apiResult.Status == StatusCodes.Status204NoContent
            ? null
            : apiResult;

        return new ObjectResult(result) { StatusCode = apiResult.Status };
    }

    public static IResult ToResult(this IApiResult apiResult)
    {
        if (apiResult is ApiResult apiResultBase && !apiResultBase.Success)
        {
            var error = apiResultBase.Errors[0];
            apiResultBase.SetStatus(error.Status);
        }

        return apiResult.Status == StatusCodes.Status204NoContent
            ? Microsoft.AspNetCore.Http.Results.NoContent()
            : Microsoft.AspNetCore.Http.Results.Json(apiResult, statusCode: apiResult.Status);
    }

    public static ApiError ToApiError(this IError error)
        => new(
            GetTitle(error),
            GetDetail(error),
            GetStatusCode(error),
            GetType(error)
        );

    public static IEnumerable<ApiError> ToApiError(this IEnumerable<IError> errors)
        => errors is null
            ? []
            : errors.Select(e => e.ToApiError());

    private static string GetDetail(IError error) =>
        error.Message;

    private static int GetStatusCode(IError error) =>
        error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

    private static string GetTitle(IError error) =>
        error.Type switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            _ => "Internal Server Error",
        };

    private static string GetType(IError error) =>
        error.Type switch
        {
            ErrorType.Validation => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
            _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        };    
}