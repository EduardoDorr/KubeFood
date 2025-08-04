using System.Collections.Concurrent;
using System.Linq.Expressions;

using KubeFood.Core.DomainEvents;

using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private const string HANDLER_ASYNC = "HandleAsync";


    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, List<Func<object, IDomainEvent, CancellationToken, Task>>> _handlerCache = [];

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();

        if (!_handlerCache.TryGetValue(eventType, out var delegates))
        {
            delegates = [];

            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var serviceHandlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in serviceHandlers)
            {
                var methodInfo = handlerType.GetMethod(HANDLER_ASYNC);

                var handlerParam = Expression.Parameter(typeof(object), "handler");
                var eventParam = Expression.Parameter(typeof(IDomainEvent), "event");
                var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                var castHandler = Expression.Convert(handlerParam, handlerType);
                var castEvent = Expression.Convert(eventParam, eventType);

                var call = Expression.Call(castHandler, methodInfo, castEvent, tokenParam);

                var lambda = Expression
                    .Lambda<Func<object, IDomainEvent, CancellationToken, Task>>(call, handlerParam, eventParam, tokenParam);

                delegates.Add(lambda.Compile());
            }

            _handlerCache[eventType] = delegates;
        }

        var handlers = _serviceProvider
            .GetServices(typeof(IDomainEventHandler<>).MakeGenericType(eventType));

        if (handlers is null)
            return;

        foreach (var handler in handlers)
            foreach (var delegateHandler in _handlerCache[eventType])
                await delegateHandler(handler!, domainEvent, cancellationToken);
    }
}