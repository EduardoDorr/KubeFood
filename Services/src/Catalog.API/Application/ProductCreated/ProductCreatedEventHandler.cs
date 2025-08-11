using KubeFood.Catalog.API.Domain.Events;
using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;

namespace KubeFood.Catalog.API.Application.ProductCreated;

public sealed class ProductCreatedEventHandler : EventHandlerBase<ProductCreatedEvent>
{
    private readonly IMessageBusProducerService _messageBusProducerService;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling {GetType().Name}");

        await _messageBusProducerService
            .PublishAsync(nameof(ProductCreatedEvent), @event, cancellationToken);
    }
}