using KubeFood.Core.DomainEvents;
using KubeFood.Core.MessageBus;

namespace KubeFood.Order.API.Application.OrderSaga.OrderRequested;

public class OrderRequestedEventHandler : IDomainEventHandler<Domain.Events.OrderRequestedEvent>
{
    private readonly ILogger<OrderRequestedEventHandler> _logger;
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderRequestedEventHandler(
        ILogger<OrderRequestedEventHandler> logger,
        IMessageBusProducerService messageBusProducerService)
    {
        _logger = logger;
        _messageBusProducerService = messageBusProducerService;
    }

    public async Task HandleAsync(Domain.Events.OrderRequestedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling {GetType().Name}");

        await _messageBusProducerService.PublishAsync(nameof(Domain.Events.OrderRequestedEvent), domainEvent, cancellationToken);
    }
}