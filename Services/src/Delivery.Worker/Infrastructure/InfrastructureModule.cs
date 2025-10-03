using KubeFood.Core;
using KubeFood.Core.Options;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Telemetry;
using KubeFood.Delivery.Worker.Application.OrderProcessing;
using KubeFood.Delivery.Worker.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;

namespace KubeFood.Delivery.Worker.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddBackgroundJobs()
            .AddMessageBus()
            .AddOptions(configuration)
            .AddOpenTelemetry(configuration);

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkerDbContext>(options =>
            options.UseMongoDB(configuration.GetConnectionString("WorkerDbConnection"), "WorkerDb"));

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<ProcessInboxMessagesJob<ObjectId, WorkerDbContext>>();
        services.AddHostedService<ProcessOutboxMessagesJob<ObjectId, WorkerDbContext>>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InboxMessageOptions>(configuration.GetSection(InboxMessageOptions.Name));
        services.Configure<OutboxMessageOptions>(configuration.GetSection(OutboxMessageOptions.Name));
        services.Configure<RabbitMqConfigurationOptions>(configuration.GetSection(RabbitMqConfigurationOptions.Name));

        return services;
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMessageBusProducer();
        services.AddMessageBusProducerOutboxService<ObjectId, WorkerDbContext>();
        services.AddMessageBusConsumerInboxService<OrderProcessingEvent, ObjectId, WorkerDbContext>(
            options => { options.QueueName = "OrderPaymentRequestedEvent"; });

        return services;
    }
}