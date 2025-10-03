using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderStockReservationRequested;

public class OrderStockReservationRequestedEventHandler : EventHandlerBase<OrderStockReservationRequestedEvent>
{
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public OrderStockReservationRequestedEventHandler(
        ILogger<OrderStockReservationRequestedEventHandler> logger,
        IMessageBusProducerOutboxService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderStockReservationRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        await _messageBusProducerService
            .PublishAsync(@event, nameof(OrderStockReservationRequestedEvent), cancellationToken);
    }
}