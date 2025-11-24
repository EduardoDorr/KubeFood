namespace KubeFood.Core.Options;

public class ApiConfigurationOptions
{
    public const string Name = "ApiConfiguration";
    public required ApiVersionOptions[] ApiVersions { get; init; }
    public required ApiContactOptions ApiContact { get; init; }
}

public class ApiVersionOptions
{
    public required string Name { get; init; }
    public required string Version { get; init; }
}

public class ApiContactOptions
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Url { get; init; }
}