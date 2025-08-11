using KubeFood.Core.Events;

namespace KubeFood.Core.MessageBus;

public interface IMessageBusConsumerEventHandler<in T> where T : IEvent
{
    Task<MessageBusConsumerEventResult> ConsumeAsync(T? message, CancellationToken cancellationToken = default);
}