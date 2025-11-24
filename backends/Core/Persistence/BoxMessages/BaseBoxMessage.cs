namespace KubeFood.Core.Persistence.BoxMessages;

public abstract class BaseBoxMessage<TId>
{
    public TId Id { get; set; }
    public string? IdempotencyId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Processed { get; set; } = false;
    public short RetryCount { get; set; } = 0;
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}