using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.Reflection;

namespace KubeFood.Core.Telemetry;

public static class SerilogExtensions
{
    public static IHostBuilder AddSerilog(this IHostBuilder host, IConfiguration configuration)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);

            var assembly = Assembly.GetEntryAssembly();
            var appName = assembly?.GetName().Name ?? "AppDesconhecida";
            var appVersion = assembly?.GetName().Version?.ToString() ?? "0.0.0";

            configuration.Enrich.WithProperty("Application", appName);
            configuration.Enrich.WithProperty("Version", appVersion);
        });

        return host;
    }
}