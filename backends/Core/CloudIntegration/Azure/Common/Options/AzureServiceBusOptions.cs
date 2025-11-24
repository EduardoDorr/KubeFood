using KubeFood.Core.Options;

using System.ComponentModel.DataAnnotations;

namespace KubeFood.Core.CloudIntegration.Azure.Common.Options;

public sealed class AzureServiceBusOptions : IOptionsSection
{
    public static string Name => "AzureServiceBus";

    [Required]
    public string ConnectionString { get; init; } = null!;
}