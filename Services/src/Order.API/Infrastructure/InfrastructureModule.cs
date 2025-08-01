using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Infrastructure.BackgroundJobs;
using KubeFood.Order.API.Infrastructure.Interceptors;
using KubeFood.Order.API.Infrastructure.Persistence;
using KubeFood.Order.API.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Order.API.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddRepositories()
            .AddBackgroundJobs();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseMySQL(
                configuration.GetConnectionString("CatalogDbConnection"))
            .AddInterceptors(new PublishDomainEventsToOutBoxMessagesInterceptor()));

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
        services.AddHostedService<ProcessOutboxMessagesJob>();

        return services;
    }
}