using KubeFood.Core.DomainEvents;
using KubeFood.Core.MessageBus;

namespace KubeFood.Order.API.Application.OrderRequested;

public class OrderRequestedHandler : IDomainEventHandler<Domain.OrderRequested>
{
    private readonly IMessageBusProducerService _messageBusProducerService;
    private readonly ILogger<OrderRequestedHandler> _logger;

    public OrderRequestedHandler(
        IMessageBusProducerService messageBusProducerService,
        ILogger<OrderRequestedHandler> logger)
    {
        _messageBusProducerService = messageBusProducerService;
        _logger = logger;
    }

    public async Task HandleAsync(Domain.OrderRequested domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling {this.GetType().Name}");

        await _messageBusProducerService.PublishAsync(nameof(Domain.OrderRequested), domainEvent, cancellationToken);
    }
}