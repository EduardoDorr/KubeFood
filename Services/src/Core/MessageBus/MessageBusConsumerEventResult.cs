namespace KubeFood.Core.MessageBus;

public enum MessageBusConsumerEventResult
{
    Ack = 0,
    Requeue = 1
}