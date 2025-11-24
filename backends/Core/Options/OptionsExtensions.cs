using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KubeFood.Core.Options;

public static class OptionsExtensions
{
    public static IServiceCollection AddValidatedOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration)
        where T : class, IOptionsSection
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(T.Name))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}