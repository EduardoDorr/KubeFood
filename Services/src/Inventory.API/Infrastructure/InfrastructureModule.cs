using KubeFood.Core;
using KubeFood.Core.Options;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Interceptors;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Telemetry;
using KubeFood.Inventory.API.Application.ItemCreated;
using KubeFood.Inventory.API.Application.ItemUpdated;
using KubeFood.Inventory.API.Application.StockReservationRequested;
using KubeFood.Inventory.API.Domain;
using KubeFood.Inventory.API.Infrastructure.Persistence;
using KubeFood.Inventory.API.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Inventory.API.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddRepositories()
            .AddBackgroundJobs()
            .AddMessageBus()
            .AddOptions(configuration)
            .AddOpenTelemetry(configuration);

        return services;
    }

    public static async Task<WebApplication> EnsureMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        await dbContext.Database.MigrateAsync();

        return app;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("InventoryDbConnection"))
            .AddInterceptors(new PublishDomainEventsToOutBoxMessagesInterceptor<int>()));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<InventoryDbContext>>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<ProcessInboxMessagesJob<int, InventoryDbContext>>();
        services.AddHostedService<ProcessOutboxMessagesJob<int, InventoryDbContext>>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InboxMessageOptions>(configuration.GetSection(InboxMessageOptions.Name));
        services.Configure<OutboxMessageOptions>(configuration.GetSection(OutboxMessageOptions.Name));
        services.Configure<OpenTelemetryOptions>(configuration.GetSection(OpenTelemetryOptions.Name));
        services.Configure<RabbitMqConfigurationOptions>(configuration.GetSection(RabbitMqConfigurationOptions.Name));

        return services;
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMessageBusProducer();
        services.AddMessageBusConsumerInboxService<ItemCreatedEvent, int, InventoryDbContext>(options
            => { options.QueueName = "ProductCreatedEvent"; });
        services.AddMessageBusConsumerInboxService<ItemUpdatedEvent, int, InventoryDbContext>(options
            => { options.QueueName = "ProductUpdatedEvent"; });
        services.AddMessageBusConsumerInboxService<StockReservationRequestedEvent, int, InventoryDbContext>(options
            => { options.QueueName = "OrderStockReservationRequestedEvent"; });

        return services;
    }
}