using KubeFood.Core.Events;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed record OutboxEvent(string Queue, object Event) : IEvent;