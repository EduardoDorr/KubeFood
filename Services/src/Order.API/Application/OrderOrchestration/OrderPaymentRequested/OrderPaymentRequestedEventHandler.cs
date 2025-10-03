using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderPaymentRequested;

public class OrderPaymentRequestedEventHandler : EventHandlerBase<OrderPaymentRequestedEvent>
{
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public OrderPaymentRequestedEventHandler(
        ILogger<OrderPaymentRequestedEventHandler> logger,
        IMessageBusProducerOutboxService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderPaymentRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling {HandlerName}", GetType().Name);

        await _messageBusProducerService
            .PublishAsync(@event, nameof(OrderPaymentRequestedEvent), cancellationToken);
    }
}