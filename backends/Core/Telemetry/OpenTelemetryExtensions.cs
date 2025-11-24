using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using System.Reflection;

namespace KubeFood.Core.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetEntryAssembly();
        var serviceName = assembly?.GetName().Name ?? "AppDesconhecida";
        var serviceVersion = assembly?.GetName().Version?.ToString() ?? "0.0.0";

        var openTelemetryOptions = new OpenTelemetryOptions();
        configuration.GetSection(OpenTelemetryOptions.Name).Bind(openTelemetryOptions);

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

        services.AddOpenTelemetry()
            .ConfigureResource(rb =>
                rb.AddService(serviceName, serviceVersion))
            .WithTracing(builder => builder
                .AddSource(serviceName)
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(configure =>
                {
                    configure.Endpoint = new Uri(openTelemetryOptions.OtlpEndpoint);
                }))
            .WithMetrics(builder => builder
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(configure =>
                {
                    configure.Endpoint = new Uri(openTelemetryOptions.OtlpEndpoint);
                }));

        return services;
    }
}

public class OpenTelemetryOptions
{
    public const string Name = "OpenTelemetryConfiguration";

    public string OtlpEndpoint { get; init; }
}