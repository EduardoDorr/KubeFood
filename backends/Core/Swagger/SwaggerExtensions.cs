using KubeFood.Core.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace KubeFood.Core.Swagger;

public static class SwaggerExtensions
{
    public static void UseCommonSwaggerDoc(this SwaggerGenOptions options, IConfiguration configuration)
    {
        var apiConfiguration = configuration
            .GetSection(ApiConfigurationOptions.Name)
            .Get<ApiConfigurationOptions>();

        ArgumentNullException.ThrowIfNull(apiConfiguration, nameof(apiConfiguration));

        foreach (var apiVersion in apiConfiguration.ApiVersions)
        {
            options.UseCommonSwaggerDoc(
                apiVersion.Name,
                apiVersion.Version,
                apiConfiguration.ApiContact.Name,
                apiConfiguration.ApiContact.Email,
                apiConfiguration.ApiContact.Url);
        }
        options.AddEnumsWithValuesFixFilters();
    }

    public static void UseCommonSwaggerDoc(this SwaggerGenOptions options, string apiName, string apiVersion, string authorName, string authorEmail, string authorUrl)
    {
        options.SwaggerDoc(apiVersion, new OpenApiInfo
        {
            Title = apiName,
            Version = apiVersion,
            Contact = new OpenApiContact
            {
                Name = authorName,
                Email = authorEmail,
                Url = new Uri(authorUrl)
            }
        });
    }

    public static void UseCommonAuthorizationBearer(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using bearer scheme."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}