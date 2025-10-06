using KubeFood.Core.CloudIntegration.Azure.Common.Factories;
using KubeFood.Core.CloudIntegration.Azure.Common.options;
using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.CloudIntegration.Azure.ServiceBus;
using KubeFood.Core.CloudIntegration.Azure.StorageAccount;
using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.CloudIntegration.Azure;

public static class AzureModule
{
    /// <summary>
    /// Registra as opções de autenticação do Azure AD e o factory de credenciais para uso com KeyVault, Storage, etc.
    /// </summary>
    public static IServiceCollection AddAzureAd(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<AzureAdOptions>(configuration);
        services.AddSingleton<IAzureCredentialFactoryService, AzureCredentialFactoryService>();

        return services;
    }

    /// <summary>
    /// Registra as opções do KeyVault.
    /// </summary>
    public static IServiceCollection AddAzureKeyVault(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<AzureKeyVaultOptions>(configuration);

        return services;
    }

    /// <summary>
    /// Registra as opções Storage Account e a service de BlobStorage.
    /// </summary>
    public static IServiceCollection AddAzureStorageAccount(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<AzureStorageAccountOptions>(configuration);
        services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();

        return services;
    }

    /// <summary>
    /// Registra as opções Service Bus.
    /// </summary>
    public static IServiceCollection AddAzureServiceBusOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<AzureServiceBusOptions>(configuration);

        return services;
    }

    /// <summary>
    /// Registra um produtor do Azure Service Bus.
    /// </summary>
    public static IServiceCollection AddAzureServiceBusProducer(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBusProducerService, AzureServiceBusProducerService>();

        return services;
    }

    /// <summary>
    /// Registra um consumidor com Inbox do Azure Service Bus.
    /// </summary>
    public static IServiceCollection AddAzureServiceBusConsumerInbox<TEvent, TId, TDbContext>(
        this IServiceCollection services,
        Action<MessageBusConsumerOptions>? consumerOptions = null)
        where TDbContext : DbContext
    {
        var options = new MessageBusConsumerOptions();
        consumerOptions?.Invoke(options);

        services.AddHostedService(sp =>
            ActivatorUtilities.CreateInstance<AzureServiceBusConsumerInboxService<TEvent, TId, TDbContext>>(sp, options)
        );

        return services;
    }

    /// <summary>
    /// Registra um consumidor do Azure Service Bus.
    /// </summary>
    public static IServiceCollection AddAzureServiceBusConsumerEvent<TEvent, THandler>(this IServiceCollection services)
        where THandler : class, IMessageBusConsumerEventHandler<TEvent>
        where TEvent : IEvent
    {
        services.AddHostedService<AzureServiceBusConsumerEventService<TEvent>>();
        services.AddScoped<IMessageBusConsumerEventHandler<TEvent>, THandler>();

        return services;
    }
}