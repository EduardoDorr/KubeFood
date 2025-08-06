using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga.OrderValidated;

public class OrderValidatedEventHandler : MessageBusConsumerServiceHandlerBase<OrderValidatedEvent>
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

    protected async override Task<MessageBusConsumerResult> ExecuteAsync(OrderValidatedEvent message, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByUniqueIdAsync(message.OrderUniqueId, cancellationToken);

        if (order is null)
        {
            _logger.LogError("Order with ID {OrderId} not found.", message.OrderUniqueId);
            return MessageBusConsumerResult.Ack;
        }

        if (message.Valid)
        {
            order.SetStatus(OrderStatus.ProductsValidated);
            order.AddItems(message.OrderItems.ToOrderItem(order.Id).ToList());

            var orderStockReservationRequestedEvent =
                new OrderStockReservationRequestedEvent(
                    order.UniqueId,
                    order.Items.Select(oi =>
                        new OrderStockReservationRequestedItemEvent(oi.ProductId, oi.Quantity)).ToList());

            order.AddDomainEvent(orderStockReservationRequestedEvent);

            _logger.LogInformation("Order with ID {OrderId} has been validated successfully.", order.Id);
        }
        else
        {
            var invalidItems = string.Join(",", message.InvalidOrderItems!);

            _logger.LogError("Order has invalid items: {InvalidItems}", invalidItems);

            order.SetStatus(OrderStatus.ProductsInvalid);
            order.SetFailureReason($"Order has invalid items: {invalidItems}");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MessageBusConsumerResult.Ack;
    }
}