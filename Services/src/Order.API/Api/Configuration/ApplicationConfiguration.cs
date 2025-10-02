using KubeFood.Core.Middlewares;
using KubeFood.Core.Swagger;
using KubeFood.Core.Telemetry;
using KubeFood.Order.API.Api.Endpoints;
using KubeFood.Order.API.Application;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Infrastructure;

using Microsoft.AspNetCore.Http.Json;

using System.Text.Json.Serialization;

using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace KubeFood.Order.API.Api.Configuration;

public static class ApplicationConfiguration
{
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Host.AddSerilog(builder.Configuration);

        builder.Services.AddDomain(builder.Configuration);
        builder.Services.AddApplicationModule();
        builder.Services.AddInfrastructureModule(builder.Configuration);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(options =>
        {
            options.UseCommonSwaggerDoc(builder.Configuration);
            options.AddEnumsWithValuesFixFilters();
        });

        return builder;
    }

    public static async Task ConfigureApplicationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.MapOrderEndpoints();

        await app.EnsureMigrationsAsync();

        await app.RunAsync();
    }
}