using KubeFood.Core.Options;

using System.ComponentModel.DataAnnotations;

namespace KubeFood.Core.CloudIntegration.Azure.Common.Options;

public sealed class AzureStorageAccountOptions : IOptionsSection
{
    public static string Name => "AzureStorageAccount";

    [Required]
    public string AccountName { get; init; } = null!;
    [Required]
    public string ContainerName { get; init; } = null!;
}