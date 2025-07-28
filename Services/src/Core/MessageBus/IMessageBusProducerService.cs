namespace KubeFood.Core.MessageBus;

public interface IMessageBusProducerService
{
    Task PublishAsync<T>(string queue, T @event, CancellationToken cancellationToken = default);
}