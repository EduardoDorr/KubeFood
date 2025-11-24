using KubeFood.Catalog.API.Application.ProductValidationRequested;
using KubeFood.Catalog.API.Domain;
using KubeFood.Catalog.API.Infrastructure.Persistence;
using KubeFood.Catalog.API.Infrastructure.Persistence.Repositories;
using KubeFood.Core;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Interceptors;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Telemetry;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddRepositories()
            .AddBackgroundJobs()
            .AddMessageBus(configuration)
            .AddOptions(configuration)
            .AddOpenTelemetry(configuration);

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseMongoDB(configuration.GetConnectionString("CatalogDbConnection"), "CatalogDb")
                .AddInterceptors(new PublishDomainEventsToOutBoxMessagesInterceptor<ObjectId>())
                .AddInterceptors(new UpdateAuditableInterceptor<ObjectId>()));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<ProcessInboxMessagesJob<ObjectId, CatalogDbContext>>();
        services.AddHostedService<ProcessOutboxMessagesJob<ObjectId, CatalogDbContext>>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InboxMessageOptions>(configuration.GetSection(InboxMessageOptions.Name));
        services.Configure<OutboxMessageOptions>(configuration.GetSection(OutboxMessageOptions.Name));
        services.Configure<OpenTelemetryOptions>(configuration.GetSection(OpenTelemetryOptions.Name));

        return services;
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBusProducerOutboxService<ObjectId, CatalogDbContext>();
        services.AddMessageBusOptions(configuration);
        services.AddMessageBusProducer(configuration);
        services.AddMessageBusConsumerInbox<ProductValidationRequestedEvent, ObjectId, CatalogDbContext>(
            configuration,
            options => { options.QueueName = "OrderRequestedEvent"; });

        return services;
    }
}