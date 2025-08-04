namespace KubeFood.Catalog.API.Application.ProcessedEvents;

public interface IProcessedEventRepository
{
    Task<bool> HasBeenProcessedAsync(string eventId, string eventType, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default);
}