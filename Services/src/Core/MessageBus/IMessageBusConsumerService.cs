namespace KubeFood.Core.MessageBus;

public interface IMessageBusConsumerService<in T>
{
    Task ConsumeAsync(T? message, CancellationToken cancellationToken = default);
}