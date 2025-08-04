using Microsoft.Extensions.Logging;

namespace KubeFood.Core.MessageBus;

public abstract class MessageBusConsumerServiceHandlerBase<T> : IMessageBusConsumerServiceHandler<T>
{
    protected readonly ILogger _logger;

    protected MessageBusConsumerServiceHandlerBase(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<MessageBusConsumerResult> ConsumeAsync(T? message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting handling the message type {Type}: {Message}", typeof(T).Name, message);

            if (message is null)
                return MessageBusConsumerResult.Ack;

            var result = await ExecuteAsync(message, cancellationToken);

            _logger.LogInformation("Finishing handling the message type {Type}: {Message}", typeof(T).Name, message);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return MessageBusConsumerResult.Requeue;
        }
    }

    protected abstract Task<MessageBusConsumerResult> ExecuteAsync(T message, CancellationToken cancellationToken = default);
}