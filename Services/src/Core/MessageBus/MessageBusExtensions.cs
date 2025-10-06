using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.CloudIntegration.Azure.ServiceBus;
using KubeFood.Core.Events;
using KubeFood.Core.Options;
using KubeFood.Core.RabbitMq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.MessageBus;

public static class MessageBusExtensions
{
    public static IServiceCollection AddMessageBusOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var setting = configuration.GetSection(MessageBusOptions.Name).Get<MessageBusOptions>()
            ?? throw new InvalidOperationException("MessageBus configuration is missing");

        services.AddValidatedOptions<MessageBusOptions>(configuration);

        switch (setting.Provider)
        {
            case MessageBusProvider.RabbitMq:
                services.AddValidatedOptions<RabbitMqOptions>(configuration);
                break;

            case MessageBusProvider.AzureServiceBus:
                services.AddValidatedOptions<AzureServiceBusOptions>(configuration);
                break;

            default:
                throw new NotSupportedException($"MessageBus provider {setting.Provider} is not supported!");
        }

        return services;
    }

    public static IServiceCollection AddMessageBusProducer(this IServiceCollection services, IConfiguration configuration)
    {
        var setting = configuration.GetSection(MessageBusOptions.Name).Get<MessageBusOptions>()
            ?? throw new InvalidOperationException("MessageBus configuration is missing");

        switch (setting.Provider)
        {
            case MessageBusProvider.RabbitMq:
                services.AddScoped<IMessageBusProducerService, RabbitMqMessageBusProducerService>();
                break;

            case MessageBusProvider.AzureServiceBus:
                services.AddScoped<IMessageBusProducerService, AzureServiceBusProducerService>();
                break;

            default:
                throw new NotSupportedException($"MessageBus provider {setting.Provider} is not supported!");
        }

        return services;
    }

    public static IServiceCollection AddMessageBusConsumerInbox<TEvent, TId, TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MessageBusConsumerOptions>? consumerOptions = null)
        where TDbContext : DbContext
    {
        var setting = configuration.GetSection(MessageBusOptions.Name).Get<MessageBusOptions>()
            ?? throw new InvalidOperationException("MessageBus configuration is missing");

        var options = new MessageBusConsumerOptions();
        consumerOptions?.Invoke(options);

        switch (setting.Provider)
        {
            case MessageBusProvider.RabbitMq:
                services.AddHostedService(sp =>
                    ActivatorUtilities.CreateInstance<RabbitMqMessageBusConsumerInboxService<TEvent, TId, TDbContext>>(sp, options)
                );
                break;

            case MessageBusProvider.AzureServiceBus:
                services.AddHostedService(sp =>
                    ActivatorUtilities.CreateInstance<AzureServiceBusConsumerInboxService<TEvent, TId, TDbContext>>(sp, options)
                );
                break;

            default:
                throw new NotSupportedException($"MessageBus provider {setting.Provider} is not supported!");
        }

        return services;
    }

    public static IServiceCollection AddMessageBusConsumerEvent<TEvent, THandler>(
        this IServiceCollection services,
        IConfiguration configuration)
        where THandler : class, IMessageBusConsumerEventHandler<TEvent>
        where TEvent : IEvent
    {
        var setting = configuration.GetSection(MessageBusOptions.Name).Get<MessageBusOptions>()
            ?? throw new InvalidOperationException("MessageBus configuration is missing");

        switch (setting.Provider)
        {
            case MessageBusProvider.RabbitMq:
                services.AddHostedService<RabbitMqMessageBusConsumerEventService<TEvent>>();
                services.AddScoped<IMessageBusConsumerEventHandler<TEvent>, THandler>();
                break;

            case MessageBusProvider.AzureServiceBus:
                services.AddHostedService<AzureServiceBusConsumerEventService<TEvent>>();
                services.AddScoped<IMessageBusConsumerEventHandler<TEvent>, THandler>();
                break;

            default:
                throw new NotSupportedException($"MessageBus provider {setting.Provider} is not supported!");
        }

        return services;
    }
}