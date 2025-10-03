using KubeFood.Core.Events;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderStatusChanged;

public sealed class OrderStatusChangedEventHandler : EventHandlerBase<OrderStatusChangedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderStatusChangedEventHandler(
        ILogger<OrderStatusChangedEventHandler> logger,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
        : base(logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
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

        if (order.Status == OrderStatus.Delivered)
        {
            order.SetStatus(OrderStatus.Completed);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}