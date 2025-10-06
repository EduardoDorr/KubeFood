using KubeFood.Core.Options;

using System.ComponentModel.DataAnnotations;

namespace KubeFood.Core.CloudIntegration.Azure.Common.options;

public sealed class AzureAdOptions : IOptionsSection
{
    public static string Name => "AzureAd";

    [Required]
    public string TenantId { get; init; } = null!;
    [Required]
    public string ClientId { get; init; } = null!;
    [Required]
    public string ClientSecret { get; init; } = null!;
}