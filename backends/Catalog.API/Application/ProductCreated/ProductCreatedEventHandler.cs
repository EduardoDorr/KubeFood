using KubeFood.Catalog.API.Domain.Events;
using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;

namespace KubeFood.Catalog.API.Application.ProductCreated;

public sealed class ProductCreatedEventHandler : EventHandlerBase<ProductCreatedEvent>
{
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger,
        IMessageBusProducerOutboxService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        await _messageBusProducerService
            .PublishAsync(@event, nameof(ProductCreatedEvent), cancellationToken);
    }
}