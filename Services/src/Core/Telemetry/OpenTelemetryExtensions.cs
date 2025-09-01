using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace KubeFood.Core.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        string serviceName,
        string serviceVersion,
        string otlpEndpoint)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(serviceName, serviceVersion))
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource(serviceName)
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(otlpEndpoint);
                }))
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddRuntimeInstrumentation()
                .SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(otlpEndpoint);
                }));

        return services;
    }





    public static IServiceCollection AddLoggingObservability(
        this IServiceCollection services, string otlpEndpoint, string serviceName, string version = "1.0.0")
    {
        services.AddLogging(logging =>
        {
            logging.AddOpenTelemetry(opt =>
            {
                opt.SetResourceBuilder(BuildResource(serviceName, version));
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            });
        });

        return services;
    }

    private static ResourceBuilder BuildResource(string serviceName, string serviceVersion)
        => ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion);
}