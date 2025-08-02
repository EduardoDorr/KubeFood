namespace KubeFood.Core.Options;

public sealed class RabbitMqConfigurationOptions
{
    public const string Name = "RabbitMqConfiguration";

    public required string HostName { get; set; }
    public required int Port { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}