using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;
using KubeFood.Order.API.Domain.Events;

namespace KubeFood.Order.API.Application.OrderSaga.OrderRequested;

public class OrderRequestedEventHandler : EventHandlerBase<OrderRequestedEvent>
{
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderRequestedEventHandler(
        ILogger<OrderRequestedEventHandler> logger,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling {GetType().Name}");

        await _messageBusProducerService
            .PublishAsync(nameof(OrderRequestedEvent), @event, cancellationToken);
    }
}