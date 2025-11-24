using Azure.Storage.Blobs;

namespace KubeFood.Core.CloudIntegration.Azure.StorageAccount;

public interface IAzureBlobStorageService
{
    Task<BlobContainerClient> GetBlobContainerClientAsync(CancellationToken cancellationToken = default);
}