using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga;

public class OrderPaymentRequestedEventHandler : EventHandlerBase<OrderPaymentRequestedEvent>
{
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderPaymentRequestedEventHandler(
        ILogger<OrderPaymentRequestedEventHandler> logger,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderPaymentRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling {HandlerName}", GetType().Name);

        await _messageBusProducerService
            .PublishAsync(nameof(OrderPaymentRequestedEvent), @event, cancellationToken);
    }
}