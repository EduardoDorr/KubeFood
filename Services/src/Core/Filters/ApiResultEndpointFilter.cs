using System.Reflection;

using KubeFood.Core.Results.Api;
using KubeFood.Core.Results.Base;
using KubeFood.Core.Results.Extensions;

using Microsoft.AspNetCore.Http;

namespace KubeFood.Core.Filters;

public sealed class ApiResultEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is null)
            return ApiResult.Ok();

        var type = result.GetType();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArg = type.GetGenericArguments()[0];

            var method = typeof(ApiResultEndpointFilter)
                .GetMethod(nameof(ProcessGenericResult), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(genericArg);

            return method.Invoke(this, [result]);
        }

        if (type == typeof(Result))
        {
            return ((Result)result).ToApiResult();
        }

        return result;
    }

    private static object ProcessGenericResult<T>(Result<T> result)
    {
        return result
            .ToApiResult()
            .ToResult();
    }
}