using KubeFood.Core.Options;

using System.ComponentModel.DataAnnotations;

namespace KubeFood.Core.CloudIntegration.Azure.Common.Options;

public sealed class AzureKeyVaultOptions : IOptionsSection
{
    public static string Name => "AzureKeyVault";

    [Required]
    public string VaultUri { get; init; } = null!;
}