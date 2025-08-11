using KubeFood.Catalog.API.Application.ValidateProducts;
using KubeFood.Catalog.API.Domain;
using KubeFood.Catalog.API.Infrastructure.Persistence;
using KubeFood.Catalog.API.Infrastructure.Persistence.Repositories;
using KubeFood.Core;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Interceptors;
using KubeFood.Core.Persistence.UnitOfWork;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration)
                .AddRepositories()
                .AddBackgroundJobs()
                .AddMessageBus();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseMongoDB(configuration.GetConnectionString("CatalogDbConnection"), "KubeFoodDb")
                .AddInterceptors(new PublishDomainEventsToOutBoxMessagesInterceptor<ObjectId>()));

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

    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMessageBusProducer();
        services.AddMessageBusConsumerInboxService<OrderRequestedEvent, ObjectId, CatalogDbContext>();

        return services;
    }
}