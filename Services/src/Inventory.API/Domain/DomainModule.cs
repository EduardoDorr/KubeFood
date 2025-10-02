using KubeFood.Core.Options;

namespace KubeFood.Inventory.API.Domain;

public static class DomainModule
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }    
}