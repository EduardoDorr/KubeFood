using Microsoft.Extensions.DependencyInjection;
using KubeFood.Core.MessageBus;
using System.Reflection;
using KubeFood.Core.DomainEvents;
using KubeFood.Core.Services;

namespace KubeFood.Core;

public static class CommonModule
{
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(IDomainEventHandler<>);

        var handlers = assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type =>
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Service = i, Implementation = type }));

        foreach (var handler in handlers)
            services.AddScoped(handler.Service, handler.Implementation);

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    public static IServiceCollection AddMessageBusProducer(this IServiceCollection services)
    {
        services.AddScoped<IMessageBusProducerService, MessageBusProducerService>();

        return services;
    }
}