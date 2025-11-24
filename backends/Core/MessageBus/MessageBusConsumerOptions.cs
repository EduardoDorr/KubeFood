namespace KubeFood.Core.MessageBus;

public sealed class MessageBusConsumerOptions
{
    public string? QueueName { get; set; }
    public string? InternalEventName { get; set; }
}