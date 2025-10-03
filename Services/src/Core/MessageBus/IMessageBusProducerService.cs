using KubeFood.Core.Events;

namespace KubeFood.Core.MessageBus;

public interface IMessageBusProducerService
{
    Task PublishAsync<TEvent>(TEvent @event, string? queue = null, CancellationToken cancellationToken = default);
}