namespace KubeFood.Core.MessageBus;

public interface IMessageBusConsumerServiceHandler<in T>
{
    Task<MessageBusConsumerResult> ConsumeAsync(T? message, CancellationToken cancellationToken = default);
}