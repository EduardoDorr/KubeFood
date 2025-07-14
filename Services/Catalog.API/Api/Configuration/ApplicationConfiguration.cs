using System.Text.Json.Serialization;

using Catalog.API.Api.Endpoints;
using Catalog.API.Application;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Persistence.Seeds;

using Core.Swagger;

using Microsoft.AspNetCore.Http.Json;

namespace Catalog.API.Api.Configuration;

public static class ApplicationConfiguration
{
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
    {
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
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapProductEndpoints();

        await app.SeedAsync();

        await app.RunAsync();
    }
}