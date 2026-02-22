using System.Reflection;

using KubeFood.Core.CommandQueries;
using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core;

public static class CommonModule
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
        => services.AddRequestHandlers(assembly).AddEventHandlers(assembly);

    public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var handlers = assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type =>
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Service = i, Implementation = type }));

        foreach (var handler in handlers)
            services.AddScoped(handler.Service, handler.Implementation);

        services.AddScoped<IRequestDispatcher, RequestDispatcher>();

        return services;
    }

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

    public static IServiceCollection AddMessageBusProducerOutboxService<TId, TDbContext>(
        this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IMessageBusProducerOutboxService, MessageBusProducerOutboxService<TId, TDbContext>>();

        if (!services.Any(sd => sd.ServiceType == typeof(IEventHandler<OutboxEvent>)))
            services.AddScoped<IEventHandler<OutboxEvent>, OutboxEventHandler>();

        return services;
    }
}