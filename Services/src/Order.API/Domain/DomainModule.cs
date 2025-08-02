using KubeFood.Core.Options;

namespace KubeFood.Order.API.Domain;

public static class DomainModule
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions(configuration);

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqConfigurationOptions>(configuration.GetSection(RabbitMqConfigurationOptions.Name));

        return services;
    }
}