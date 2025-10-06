using KubeFood.Core.Options;

using System.ComponentModel.DataAnnotations;

namespace KubeFood.Core.RabbitMq;

public sealed class RabbitMqOptions : IOptionsSection
{
    public static string Name => "RabbitMqConfiguration";

    [Required]
    public string HostName { get; set; } = null!;
    [Required]
    public int Port { get; set; }
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}