using System.Net;

using KubeFood.Core.Results.Api;
using KubeFood.Core.Results.Errors;
using KubeFood.Core.Results.Extensions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KubeFood.Core.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Exception occurred: {Message}",
            exception.Message);

        var errors =
            new List<IError>
            {
                new Error(
                    "Exception occurred",
                    exception.Message,
                    ErrorType.Failure)
            };

        var result =
            new ApiResult(
                HttpStatusCode.InternalServerError,
                errors.ToApiError())
            .ToActionResult();

        httpContext.Response.StatusCode = result.StatusCode!.Value;

        await httpContext.Response
            .WriteAsJsonAsync(result.Value, cancellationToken);

        return true;
    }
}