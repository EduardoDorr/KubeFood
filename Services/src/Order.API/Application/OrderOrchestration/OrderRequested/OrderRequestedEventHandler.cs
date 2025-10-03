using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Order.API.Domain;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderRequested;

public class OrderRequestedEventHandler : EventHandlerBase<OrderRequestedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public OrderRequestedEventHandler(
        ILogger<OrderRequestedEventHandler> logger,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMessageBusProducerOutboxService messageBusProducerOutboxService)
        : base(logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _messageBusProducerService = messageBusProducerOutboxService;
    }

    protected override async Task ExecuteAsync(OrderRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByUniqueIdAsync(@event.Id, cancellationToken);

        if (order is null)
        {
            _logger.LogError("Order with ID {OrderId} not found.", @event.Id);
            return;
        }

        order.SetStatus(OrderStatus.ProductsValidation);

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _messageBusProducerService
            .PublishAsync(@event, nameof(OrderRequestedEvent), cancellationToken);
    }
}