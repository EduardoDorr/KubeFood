using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga;

public class OrderRequestedEventHandler : EventHandlerBase<OrderRequestedEvent>
{
    private readonly IMessageBusProducerOutboxService _messageBusProducerOutboxService;

    public OrderRequestedEventHandler(
        ILogger<OrderRequestedEventHandler> logger,
        IMessageBusProducerOutboxService messageBusProducerOutboxService)
        : base(logger)
    {
        _messageBusProducerOutboxService = messageBusProducerOutboxService;
    }

    protected override async Task ExecuteAsync(OrderRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        await _messageBusProducerOutboxService
            .PublishAsync(@event, nameof(OrderRequestedEvent), cancellationToken);
    }
}