using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;

using Microsoft.Extensions.Logging;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class OutboxEventHandler : EventHandlerBase<OutboxEvent>
{
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OutboxEventHandler(
        ILogger<OutboxEventHandler> logger,
        IMessageBusProducerService messageBusProducerService) : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OutboxEvent @event, CancellationToken cancellationToken = default)
    {
        await _messageBusProducerService
            .PublishAsync(@event.Queue, @event.Event, cancellationToken);
    }
}