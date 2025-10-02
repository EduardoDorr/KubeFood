using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga;

public class OrderStockReservationRequestedEventHandler : EventHandlerBase<OrderStockReservationRequestedEvent>
{
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderStockReservationRequestedEventHandler(
        ILogger<OrderStockReservationRequestedEventHandler> logger,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderStockReservationRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        await _messageBusProducerService
            .PublishAsync(nameof(OrderStockReservationRequestedEvent), @event, cancellationToken);
    }
}