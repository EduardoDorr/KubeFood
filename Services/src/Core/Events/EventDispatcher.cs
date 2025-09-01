using System.Collections.Concurrent;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.Events;

public class EventDispatcher : IEventDispatcher
{
    private const string HANDLER_ASYNC = "HandleAsync";

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<Type, List<Func<object, IEvent, CancellationToken, Task>>> _handlerCache = [];

    public EventDispatcher(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task DispatchAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var dispatchScope = _serviceScopeFactory.CreateAsyncScope();
        var eventType = @event.GetType();

        if (!_handlerCache.TryGetValue(eventType, out var delegates))
        {
            delegates = [];

            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());

            var serviceHandlers = dispatchScope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in serviceHandlers)
            {
                var methodInfo = handlerType.GetMethod(HANDLER_ASYNC);

                if (methodInfo is null)
                    continue;

                var handlerParam = Expression.Parameter(typeof(object), "handler");
                var eventParam = Expression.Parameter(typeof(IEvent), "event");
                var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                var castHandler = Expression.Convert(handlerParam, handlerType);
                var castEvent = Expression.Convert(eventParam, eventType);

                var call = Expression.Call(castHandler, methodInfo, castEvent, tokenParam);

                var lambda = Expression
                    .Lambda<Func<object, IEvent, CancellationToken, Task>>(call, handlerParam, eventParam, tokenParam);

                delegates.Add(lambda.Compile());
            }

            _handlerCache[eventType] = delegates;
        }

        var handlers = dispatchScope.ServiceProvider
            .GetServices(typeof(IEventHandler<>).MakeGenericType(eventType));

        if (handlers is null)
            return;

        foreach (var handler in handlers)
            foreach (var delegateHandler in _handlerCache[eventType])
                await delegateHandler(handler!, @event, cancellationToken);
    }
}