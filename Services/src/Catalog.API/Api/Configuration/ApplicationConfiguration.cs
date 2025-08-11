using System.Text.Json.Serialization;

using KubeFood.Catalog.API.Api.Endpoints;
using KubeFood.Catalog.API.Application;
using KubeFood.Catalog.API.Domain;
using KubeFood.Catalog.API.Infrastructure;
using KubeFood.Catalog.API.Infrastructure.Persistence.Seeds;
using KubeFood.Core.Middlewares;
using KubeFood.Core.Swagger;

using Microsoft.AspNetCore.Http.Json;

namespace KubeFood.Catalog.API.Api.Configuration;

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
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.MapProductEndpoints();

        await app.SeedAsync();

        await app.RunAsync();
    }
}