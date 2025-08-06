using MongoDB.Bson;

namespace KubeFood.Catalog.API.Application.ProcessedEvents;

public sealed class ProcessedEvent
{
    public ObjectId Id { get; private set; }
    public string EventId { get; private set; }
    public string EventType { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private ProcessedEvent() { }

    public ProcessedEvent(
        string eventId,
        string eventType)
    {
        EventId = eventId;
        EventType = eventType;
        ProcessedAt = DateTime.Now;
    }
}