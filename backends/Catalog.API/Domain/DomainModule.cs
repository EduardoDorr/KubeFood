using KubeFood.Core.RabbitMq;

namespace KubeFood.Catalog.API.Domain;

public static class DomainModule
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions(configuration);

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Name));

        return services;
    }
}