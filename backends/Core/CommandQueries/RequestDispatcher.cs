using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.CommandQueries;

public sealed class RequestDispatcher : IRequestDispatcher
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<Type, Func<IServiceProvider, IRequest<object>, CancellationToken, Task<object>>> _handlerCache = [];

    public RequestDispatcher(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResult> DispatchAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        var requestType = request.GetType();

        if (!_handlerCache.TryGetValue(requestType, out var executor))
        {
            executor = BuildExecutor(requestType, typeof(TResult));
            _handlerCache[requestType] = executor;
        }

        var result = await executor(scope.ServiceProvider, (IRequest<object>)request, cancellationToken);

        return (TResult)result!;
    }

    private static Func<IServiceProvider, IRequest<object>, CancellationToken, Task<object>> BuildExecutor(Type requestType, Type resultType)
    {
        return async (sp, req, ct) =>
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, resultType);
            dynamic handler = sp.GetRequiredService(handlerType);

            return await handler.HandleAsync((dynamic)req, ct);
        };
    }
}