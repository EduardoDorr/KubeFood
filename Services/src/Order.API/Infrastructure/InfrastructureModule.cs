using KubeFood.Core;
using KubeFood.Core.Options;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Interceptors;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Telemetry;
using KubeFood.Order.API.Application.OrderSaga.OrderStockReserved;
using KubeFood.Order.API.Application.OrderSaga.OrderValidatedEvent;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Infrastructure.Persistence;
using KubeFood.Order.API.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;

namespace KubeFood.Order.API.Infrastructure;

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
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        await dbContext.Database.MigrateAsync();

        return app;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseMySQL(
                configuration.GetConnectionString("OrderDbConnection"))
            .AddInterceptors(new PublishDomainEventsToOutBoxMessagesInterceptor<int>()));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<ProcessInboxMessagesJob<int, OrderDbContext>>();
        services.AddHostedService<ProcessOutboxMessagesJob<int, OrderDbContext>>();

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
        services.AddMessageBusProducerOutboxService<int, OrderDbContext>();
        services.AddMessageBusConsumerInboxService<OrderValidatedEvent, int, OrderDbContext>(options
            => { options.QueueName = "ProductValidatedEvent"; });
        services.AddMessageBusConsumerInboxService<OrderStockReservedEvent, int, OrderDbContext>(options
            => { options.QueueName = "StockReservedEvent"; });

        return services;
    }
}