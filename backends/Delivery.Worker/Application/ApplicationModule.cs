using KubeFood.Core;
using KubeFood.Delivery.Worker.Application;

using System.Reflection;

namespace KubeFood.Delivery.Worker.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services
            .AddEventHandlers();

        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddEventHandlers(Assembly.GetExecutingAssembly());

        return services;
    }
}