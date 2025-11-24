using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.RabbitMq;

public static class RabbitMqMessageBusExtensions
{
    public static IServiceCollection AddRabbitMqOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<RabbitMqOptions>(configuration);

        return services;
    }

    public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services)
    {
        services.AddScoped<IMessageBusProducerService, RabbitMqMessageBusProducerService>();

        return services;
    }

    public static IServiceCollection AddRabbitMqConsumerInbox<TEvent, TId, TDbContext>(
        this IServiceCollection services,
        Action<MessageBusConsumerOptions>? consumerOptions = null)
        where TDbContext : DbContext
    {
        var options = new MessageBusConsumerOptions();
        consumerOptions?.Invoke(options);

        services.AddHostedService(sp =>
            ActivatorUtilities.CreateInstance<RabbitMqMessageBusConsumerInboxService<TEvent, TId, TDbContext>>(sp, options)
        );

        return services;
    }

    public static IServiceCollection AddRabbitMqConsumerEvent<TEvent, THandler>(this IServiceCollection services)
        where THandler : class, IMessageBusConsumerEventHandler<TEvent>
        where TEvent : IEvent
    {
        services.AddHostedService<RabbitMqMessageBusConsumerEventService<TEvent>>();
        services.AddScoped<IMessageBusConsumerEventHandler<TEvent>, THandler>();

        return services;
    }
}