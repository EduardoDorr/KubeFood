using Microsoft.Extensions.DependencyInjection;
using KubeFood.Core.MessageBus;
using System.Reflection;
using KubeFood.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace KubeFood.Core;

public static class CommonModule
{
    public static IServiceCollection AddEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(IEventHandler<>);

        var handlers = assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type =>
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Service = i, Implementation = type }));

        foreach (var handler in handlers)
            services.AddScoped(handler.Service, handler.Implementation);

        services.AddSingleton<IEventDispatcher, EventDispatcher>();

        return services;
    }

    public static IServiceCollection AddMessageBusProducer(this IServiceCollection services)
    {
        services.AddScoped<IMessageBusProducerService, MessageBusProducerService>();

        return services;
    }

    public static IServiceCollection AddMessageBusConsumerInboxService<TEvent, TId, TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddHostedService<MessageBusConsumerInboxService<TEvent, TId, TDbContext>>();

        return services;
    }

    public static IServiceCollection AddMessageBusConsumerEvent<TEvent, THandler>(this IServiceCollection services)
        where THandler : class, IMessageBusConsumerEventHandler<TEvent>
        where TEvent : IEvent
    {
        services.AddHostedService<MessageBusConsumerEvent<TEvent>>();
        services.AddScoped<IMessageBusConsumerEventHandler<TEvent>, THandler>();

        return services;
    }
}