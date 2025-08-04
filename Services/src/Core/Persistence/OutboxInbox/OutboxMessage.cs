namespace KubeFood.Core.Persistence.OutboxInbox;

public sealed class OutboxMessage<TId>
{
    public TId Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Processed { get; set; } = false;
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}