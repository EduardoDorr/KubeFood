using KubeFood.Core.Events;

using Microsoft.Extensions.Logging;

namespace KubeFood.Core.MessageBus;

public abstract class MessageBusConsumerEventHandlerBase<T> : IMessageBusConsumerEventHandler<T> 
    where T : IEvent
{
    protected readonly ILogger _logger;

    protected MessageBusConsumerEventHandlerBase(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<MessageBusConsumerEventResult> ConsumeAsync(T? message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting handling the message type {Type}: {Message}", typeof(T).Name, message);

            if (message is null)
                return MessageBusConsumerEventResult.Ack;

            var result = await ExecuteAsync(message, cancellationToken);

            _logger.LogInformation("Finishing handling the message type {Type}: {Message}", typeof(T).Name, message);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return MessageBusConsumerEventResult.Requeue;
        }
    }

    protected abstract Task<MessageBusConsumerEventResult> ExecuteAsync(T message, CancellationToken cancellationToken = default);
}