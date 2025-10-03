using KubeFood.Catalog.API.Domain.Events;
using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;

namespace KubeFood.Catalog.API.Application.ProductUpdated;

public sealed class ProductUpdatedEventHandler : EventHandlerBase<ProductUpdatedEvent>
{
    private readonly IMessageBusProducerOutboxService _messageBusProducerService;

    public ProductUpdatedEventHandler(
        ILogger<ProductUpdatedEventHandler> logger,
        IMessageBusProducerOutboxService messageBusProducerService)
        : base(logger)
    {
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(ProductUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        await _messageBusProducerService
            .PublishAsync(@event, nameof(ProductUpdatedEvent), cancellationToken);
    }
}