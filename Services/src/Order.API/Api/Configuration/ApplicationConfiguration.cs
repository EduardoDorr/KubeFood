using System.Text.Json.Serialization;

using KubeFood.Core;
using KubeFood.Core.Swagger;
using KubeFood.Order.API.Api.Endpoints;
using KubeFood.Order.API.Application;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Infrastructure;

using Microsoft.AspNetCore.Http.Json;

using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace KubeFood.Order.API.Api.Configuration;

public static class ApplicationConfiguration
{
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMessageBusProducer();
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
            options.AddEnumsWithValuesFixFilters();
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

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.MapOrderEndpoints();

        await app.RunAsync();
    }
}