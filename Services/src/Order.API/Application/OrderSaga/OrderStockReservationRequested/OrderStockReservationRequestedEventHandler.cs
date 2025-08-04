using KubeFood.Core.DomainEvents;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga.OrderStockReservationRequested;

public class OrderStockReservationRequestedEventHandler : IDomainEventHandler<OrderStockReservationRequestedEvent>
{
    private readonly ILogger<OrderStockReservationRequestedEventHandler> _logger;
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderStockReservationRequestedEventHandler(
        ILogger<OrderStockReservationRequestedEventHandler> logger,
        IMessageBusProducerService messageBusProducerService)
    {
        _logger = logger;
        _messageBusProducerService = messageBusProducerService;
    }

    public async Task HandleAsync(OrderStockReservationRequestedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling {GetType().Name}");

        await _messageBusProducerService.PublishAsync(nameof(OrderStockReservationRequestedEvent), domainEvent, cancellationToken);
    }
}