using System.Text.Json.Serialization;

using KubeFood.Core.Swagger;
using KubeFood.Inventory.API.Api.Endpoints;
using KubeFood.Inventory.API.Application;
using KubeFood.Inventory.API.Domain;
using KubeFood.Inventory.API.Infrastructure;

using Microsoft.AspNetCore.Http.Json;

namespace KubeFood.Inventory.API.Api.Configuration;

public static class ApplicationConfiguration
{
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDomain(builder.Configuration);
        builder.Services.AddApplicationModule();
        builder.Services.AddInfrastructureModule(builder.Configuration);

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(options =>
        {
            options.UseCommonSwaggerDoc(builder.Configuration);
        });

        return builder;
    }

    public static async Task ConfigureApplicationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwagger();
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.MapInventoryEndpoints();

        await app.EnsureMigrationsAsync();

        await app.RunAsync();
    }
}