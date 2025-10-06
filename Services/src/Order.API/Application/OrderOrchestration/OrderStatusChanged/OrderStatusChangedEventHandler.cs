using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderStatusChanged;

public sealed class OrderStatusChangedEventHandler : EventHandlerBase<OrderStatusChangedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public OrderStatusChangedEventHandler(
        ILogger<OrderStatusChangedEventHandler> logger,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMessageBusProducerOutboxService messageBusProducerService)
        : base(logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderStatusChangedEvent @event, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByUniqueIdAsync(@event.Id, cancellationToken);

        if (order is null)
        {
            _logger.LogError("Order with ID {OrderId} not found.", @event.Id);
            return;
        }

        order.SetStatus(@event.Status);
        _logger.LogInformation("Order with Id {OrderId} has its status changed to {Status}.", order.Id, order.Status);

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (order.Status == OrderStatus.InPreparation)
        {
            var orderStockReservationConsumedEvent =
                new OrderStockReservationConsumedEvent(
                    order.UniqueId,
                    order.Items
                        .Select(i => new OrderStockReservationConsumedItemEvent(i.ProductId, i.Quantity))
                        .ToList());

            await _messageBusProducerService
                .PublishAsync(
                    orderStockReservationConsumedEvent,
                    nameof(OrderStockReservationConsumedEvent),
                    cancellationToken);
        }

        if (order.Status == OrderStatus.Delivered)
        {
            order.SetStatus(OrderStatus.Completed);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}