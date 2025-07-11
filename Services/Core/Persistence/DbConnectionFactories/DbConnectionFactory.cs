using Microsoft.Extensions.Configuration;
using Core.Persistence.DbConnectionFactories;

namespace Core.Persistence.DbConnectionFactories;

public static class DbConnectionFactory
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        if (Environment.GetEnvironmentVariable("DOCKER_ENVIROMENT") == "DockerDevelopment")
            return configuration.GetConnectionString("ContainerConnection")!;

        return configuration.GetConnectionString("LocalConnection")!;
    }
}