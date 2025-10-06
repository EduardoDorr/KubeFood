using Azure.Storage.Blobs;

using KubeFood.Core.CloudIntegration.Azure.Common.Factories;
using KubeFood.Core.CloudIntegration.Azure.Common.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KubeFood.Core.CloudIntegration.Azure.StorageAccount;

public sealed class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly IAzureCredentialFactoryService _azureCredentialFactory;
    private readonly AzureStorageAccountOptions _storageOptions;

    public AzureBlobStorageService(
        IAzureCredentialFactoryService azureCredentialFactory,
        IOptions<AzureStorageAccountOptions> storageOptions)
    {
        _azureCredentialFactory = azureCredentialFactory;
        _storageOptions = storageOptions.Value;
    }

    public async Task<BlobContainerClient> GetBlobContainerClientAsync(CancellationToken cancellationToken = default)
    {
        var credential = _azureCredentialFactory
            .GetTokenCredential();

        var blobServiceClient = new BlobServiceClient(
           new Uri($"https://{_storageOptions.AccountName}.blob.core.windows.net"),
           credential);

        var containerClient = blobServiceClient
            .GetBlobContainerClient(_storageOptions.ContainerName);

        await containerClient
            .CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        return containerClient;
    }
}