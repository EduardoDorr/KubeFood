using Azure.Identity;
using Azure.Storage.Blobs;

using Microsoft.Extensions.Configuration;

namespace KubeFood.Core.CloudIntegration.Azure.StorageAccount;

public static class AzureStorageAccountExtensions
{
    public static async Task<BlobContainerClient> GetBlobContainerClientAsync(IConfiguration configuration)
    {
        var tenantId = configuration["AzureAd:TenantId"];
        var clientId = configuration["AzureAd:ClientId"];
        var clientSecret = configuration["AzureAd:ClientSecret"];
        var storageAccountName = configuration["StorageAccount:AccountName"];

        var credential =
            new ClientSecretCredential(
                tenantId!,
                clientId!,
                clientSecret!);

        var blobServiceClient =
            new BlobServiceClient(
                new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                credential);

        var containerName = configuration["StorageAccount:ContainerName"];

        var containerClient = blobServiceClient
            .GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }
}