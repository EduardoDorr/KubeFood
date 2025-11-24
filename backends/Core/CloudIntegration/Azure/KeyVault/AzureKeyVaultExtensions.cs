using KubeFood.Core.CloudIntegration.Azure.Common;
using KubeFood.Core.CloudIntegration.Azure.Common.Factories;
using KubeFood.Core.CloudIntegration.Azure.KeyVault;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KubeFood.Core.CloudIntegration.Azure.KeyVault;

public static class AzureKeyVaultExtension
{
    public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, ILogger logger)
    {
        logger.LogTrace("Adding Azure Key Vault");

        var keyVaultUri = configuration[AzureConstants.KEY_VAULT_URI]
            ?? throw new ArgumentNullException(AzureConstants.KEY_VAULT_URI);

        var credential = AzureCredentialFactory.Create(configuration, logger);

        configurationBuilder.AddAzureKeyVault(
            new Uri(keyVaultUri),
            credential);

        return configurationBuilder;
    }
}