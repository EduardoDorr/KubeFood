using KubeFood.Core.Events;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderValidatedEvent;

public class OrderValidatedEventHandler : EventHandlerBase<OrderValidatedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderValidatedEventHandler(
        ILogger<OrderValidatedEventHandler> logger,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
        : base(logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ExecuteAsync(OrderValidatedEvent @event, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByUniqueIdAsync(@event.Id, cancellationToken);

        if (order is null)
        {
            _logger.LogError("Order with ID {OrderId} not found.", @event.Id);
            return;
        }

        if (@event.Valid)
        {
            order.SetStatus(OrderStatus.StockValidation);
            order.AddItems(@event.Items.ToOrderItem(order.Id).ToList());

            var orderStockReservationRequestedEvent =
                new OrderStockReservationRequestedEvent(
                    order.UniqueId,
                    order.Items.Select(item =>
                        new OrderStockReservationRequestedItemEvent(item.ProductId, item.Quantity)).ToList());

            order.AddDomainEvent(orderStockReservationRequestedEvent);

            _logger.LogInformation("Order with ID {OrderId} has been validated successfully.", order.Id);
        }
        else
        {
            var invalidItems = string.Join(",", @event.InvalidItems!);

            _logger.LogError("Order has invalid items: {InvalidItems}", invalidItems);

            order.SetStatus(OrderStatus.ProductsInvalid);
            order.SetFailureReason($"Order has invalid items: {invalidItems}");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}