using KubeFood.Core.Events;

namespace KubeFood.Core.MessageBus;

public interface IMessageBusProducerOutboxService
{
    Task PublishAsync<TEvent>(TEvent @event, string? queue = null, CancellationToken cancellationToken = default) where TEvent : IEvent;
}