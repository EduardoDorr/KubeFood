using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Delivery.Worker.Domain;

namespace KubeFood.Delivery.Worker.Application.OrderProcessing;

public sealed class OrderProcessingEventHandler : EventHandlerBase<OrderProcessingEvent>
{
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public OrderProcessingEventHandler(
        ILogger<OrderProcessingEventHandler> logger,
        IMessageBusProducerOutboxService messageBusProducerOutboxService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerOutboxService;
    }

    protected override async Task ExecuteAsync(OrderProcessingEvent @event, CancellationToken cancellationToken = default)
    {
        // 1. Await for payment confirmation
        await Task.Delay(10000, cancellationToken);

        var orderStatusEvent =
            new OrderStatusChangedEvent(@event.Id, OrderStatus.PaymentConfirmed);

        await _messageBusProducerService
            .PublishAsync(orderStatusEvent, nameof(OrderStatusChangedEvent), cancellationToken);

        // 2. Await for starting preparation
        await Task.Delay(10000, cancellationToken);

        orderStatusEvent =
            new OrderStatusChangedEvent(@event.Id, OrderStatus.InPreparation);

        await _messageBusProducerService
            .PublishAsync(orderStatusEvent, nameof(OrderStatusChangedEvent), cancellationToken);

        // 3. Await for order ready for delivery
        await Task.Delay(10000, cancellationToken);

        orderStatusEvent =
            new OrderStatusChangedEvent(@event.Id, OrderStatus.ReadyForDelivery);

        await _messageBusProducerService
            .PublishAsync(orderStatusEvent, nameof(OrderStatusChangedEvent), cancellationToken);

        // 4. Await for order delivered
        await Task.Delay(10000, cancellationToken);

        orderStatusEvent =
            new OrderStatusChangedEvent(@event.Id, OrderStatus.Delivered);

        await _messageBusProducerService
            .PublishAsync(orderStatusEvent, nameof(OrderStatusChangedEvent), cancellationToken);
    }
}