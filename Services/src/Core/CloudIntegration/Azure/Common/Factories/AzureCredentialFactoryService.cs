using Azure.Core;
using Azure.Identity;

using KubeFood.Core.CloudIntegration.Azure.Common.options;
using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.CloudIntegration.Azure.StorageAccount;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KubeFood.Core.CloudIntegration.Azure.Common.Factories;

public sealed class AzureCredentialFactoryService : IAzureCredentialFactoryService
{
    private readonly ILogger<AzureCredentialFactoryService> _logger;
    private readonly IHostEnvironment _environment;
    private readonly AzureAdOptions _azureAdOptions;

    public AzureCredentialFactoryService(
        ILogger<AzureCredentialFactoryService> logger,
        IHostEnvironment environment,
        IOptions<AzureAdOptions> azureAdOptions)
    {
        _logger = logger;
        _environment = environment;
        _azureAdOptions = azureAdOptions.Value;
    }

    public TokenCredential GetTokenCredential()
    {
        if (_environment.IsDevelopment())
        {
            _logger.LogTrace("Using DefaultAzureCredential for Development");

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

        _logger.LogTrace("Using ClientSecretCredential for non-development environment");

        return new ClientSecretCredential(
            _azureAdOptions.TenantId,
            _azureAdOptions.ClientId,
            _azureAdOptions.ClientSecret);
    }
}