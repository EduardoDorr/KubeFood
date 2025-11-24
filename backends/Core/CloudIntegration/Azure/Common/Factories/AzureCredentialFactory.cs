using Azure.Core;
using Azure.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KubeFood.Core.CloudIntegration.Azure.Common.Factories;

public static class AzureCredentialFactory
{
    public static TokenCredential Create(IConfiguration configuration, ILogger logger)
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";

        if (environment == "Development")
        {
            logger.LogTrace("Using DefaultAzureCredential for Development");

            return new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    ExcludeEnvironmentCredential = false,
                    ExcludeManagedIdentityCredential = false,
                    ExcludeVisualStudioCodeCredential = false,
                    ExcludeVisualStudioCredential = false,
                    ExcludeAzureCliCredential = false,
                    ExcludeInteractiveBrowserCredential = true
                });
        }

        logger.LogTrace("Using ClientSecretCredential for Production");

        var tenantId = configuration[AzureConstants.TENANT_ID]
            ?? throw new ArgumentNullException(AzureConstants.TENANT_ID);

        var clientId = configuration[AzureConstants.CLIENT_ID]
            ?? throw new ArgumentNullException(AzureConstants.CLIENT_ID);

        var clientSecret = configuration[AzureConstants.CLIENT_SECRET]
            ?? throw new ArgumentNullException(AzureConstants.CLIENT_SECRET);

        return new ClientSecretCredential(tenantId, clientId, clientSecret);
    }
}