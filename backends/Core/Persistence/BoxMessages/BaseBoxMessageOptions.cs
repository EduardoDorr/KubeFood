namespace KubeFood.Core.Persistence.BoxMessages;

public abstract class BaseBoxMessageOptions
{
    public required int RetryLimit { get; init; } = 5;
    public required int BatchSize { get; init; } = 20;
    public required int ProcessIntervalInSeconds { get; init; } = 60;
}