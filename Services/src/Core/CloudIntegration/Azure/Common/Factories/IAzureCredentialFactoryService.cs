using Azure.Core;

namespace KubeFood.Core.CloudIntegration.Azure.Common.Factories;

public interface IAzureCredentialFactoryService
{
    TokenCredential GetTokenCredential();
}