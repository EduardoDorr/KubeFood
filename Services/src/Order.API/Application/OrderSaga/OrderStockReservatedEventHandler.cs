using KubeFood.Core.Events;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga;

public class OrderStockReservatedEventHandler : EventHandlerBase<OrderStockReservatedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderStockReservatedEventHandler(
        ILogger<OrderStockReservatedEventHandler> logger,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
        : base(logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ExecuteAsync(OrderStockReservatedEvent @event, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByUniqueIdAsync(@event.OrderUniqueId, cancellationToken);

        if (order is null)
        {
            _logger.LogError("Order with ID {OrderId} not found.", @event.OrderUniqueId);
            return;
        }

        if (@event.IsStockReservated)
        {
            order.SetStatus(OrderStatus.StockAvailable);

            var orderPaymentRequestedEvent =
                new OrderPaymentRequestedEvent(
                    order.UniqueId);

            order.AddDomainEvent(orderPaymentRequestedEvent);

            _logger.LogInformation("Order with Id {OrderId} has all items available.", order.Id);
        }
        else
        {
            var unavailableItems = string.Join(",", @event.UnavailableItems!);

            _logger.LogError("Order has unavailable items: {UnavailableItems}", unavailableItems);

            order.SetStatus(OrderStatus.StockUnavailable);
            order.SetFailureReason($"Order has unavailable items: {unavailableItems}");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}