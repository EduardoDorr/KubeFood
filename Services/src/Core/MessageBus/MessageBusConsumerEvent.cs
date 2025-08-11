using KubeFood.Core.Events;
using KubeFood.Core.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KubeFood.Core.MessageBus;

public class MessageBusConsumerEvent<T> : MessageBusConsumerBase<T>
    where T : IEvent
{
    private readonly IServiceProvider _serviceProvider;

    public MessageBusConsumerEvent(
        IServiceProvider serviceProvider,
        ILogger<MessageBusConsumerEvent<T>> logger,
        IOptions<RabbitMqConfigurationOptions> rabbitMqConfigurationOptions)
        : base(logger, rabbitMqConfigurationOptions)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleMessageAsync(T message, IChannel channel, BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<IMessageBusConsumerEventHandler<T>>();

        var result = await handler.ConsumeAsync(message, cancellationToken);

        if (result == MessageBusConsumerEventResult.Ack)
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        else
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, cancellationToken);
    }
}