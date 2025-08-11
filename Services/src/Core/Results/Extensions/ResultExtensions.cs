using KubeFood.Core.Results.Base;
using KubeFood.Core.Results.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KubeFood.Core.Results.Extensions;

public static class ResultExtensions
{
    public static T Match<T>(this Result result, Func<T> onSuccess, Func<Result, T> onFailure)
    {
        return result.Success ? onSuccess() : onFailure(result);
    }

    public static T Match<T, TValue>(this Result<TValue> result, Func<TValue?, T> onSuccess, Func<Result<TValue>, T> onFailure)
    {
        return result.Success ? onSuccess(result.Value) : onFailure(result);
    }

    public static ProblemDetails ToProblemDetails(this IError error)
        => new()
        {
            Detail = GetDetail(error),
            Status = GetStatusCode(error.Type),
            Title = GetTitle(error.Type),
            Type = GetType(error.Type)
        };

    public static IEnumerable<ProblemDetails> ToProblemDetails(this IEnumerable<IError> errors)
        => errors is null
            ? []
            : errors.Select(e => e.ToProblemDetails());

    public static IResult ToProblemDetails(this IResultBase result)
    {
        if (result.Success)
            throw new InvalidOperationException("Result is a success!");

        var error = result.Errors[0];

        var problemDetails = error.ToProblemDetails();

        return Microsoft.AspNetCore.Http.Results.Problem(problemDetails);
    }

    private static string GetDetail(IError error) =>
        error.Message;

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

    private static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            _ => "Internal Server Error",
        };

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
            _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        };
}