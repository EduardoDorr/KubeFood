using Catalog.API.Domain;
using Catalog.API.Infrastructure.Persistence;
using Catalog.API.Infrastructure.Persistence.Repositories;

using Core.Persistence.UnitOfWork;

using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseMongoDB(configuration.GetConnectionString("CatalogDbConnection"), "KubeFoodDb"));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}