using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace KubeFood.Core.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthenticationBearer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Authentication:Authority"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Authentication:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["Authentication:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),

                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,

                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
                };
            });

        services.AddAuthorization();

        return services;
    }
}