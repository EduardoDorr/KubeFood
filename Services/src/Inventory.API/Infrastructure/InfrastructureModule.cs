using KubeFood.Core;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Interceptors;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Inventory.API.Domain;
using KubeFood.Inventory.API.Domain.Events;
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
            .AddMessageBus();

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

    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMessageBusProducer();
        services.AddMessageBusConsumerInboxService<OrderStockReservationRequestedEvent, int, InventoryDbContext>();

        return services;
    }
}