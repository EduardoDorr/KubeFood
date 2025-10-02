using KubeFood.Catalog.API.Domain;
using KubeFood.Catalog.API.Domain.Events;
using KubeFood.Core.Events;
using KubeFood.Core.Helpers;
using KubeFood.Core.MessageBus;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Application.ProductValidationRequested;

public class ProductValidationRequestedEventHandler : EventHandlerBase<ProductValidationRequestedEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBusProducerService _messageBusProducerService;

    public ProductValidationRequestedEventHandler(
        ILogger<ProductValidationRequestedEventHandler> logger,
        IProductRepository productRepository,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _productRepository = productRepository;
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(ProductValidationRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        ProductValidatedEvent productValidatedEvent;

        var objectIds = @event
            .Items
            .Select(o => o.Id.DecodeHashId<ObjectId>());

        if (objectIds is null)
        {
            productValidatedEvent =
                new ProductValidatedEvent(
                    @event!.Id,
                    false,
                    [],
                    @event.Items.Select(o => o.Id).ToList());
        }
        else
        {
            var products = await _productRepository
                .GetByIdsAsync(objectIds!, cancellationToken);

            var items = products
                .ToOrderItem(@event.Items);

            if (objectIds!.Count() > products.Count())
            {
                var invalidItems = @event.Items
                    .Where(oi => products.All(p => p.Id != oi.Id.DecodeHashId<ObjectId>()))
                    .ToList();

                productValidatedEvent =
                    new ProductValidatedEvent(
                        @event!.Id,
                        false,
                        items,
                        invalidItems.Select(i => i.Id).ToList());
            }
            else
            {
                productValidatedEvent =
                    new ProductValidatedEvent(
                        @event.Id,
                        true,
                        items);
            }
        }

        await _messageBusProducerService
            .PublishAsync(nameof(ProductValidatedEvent), productValidatedEvent, cancellationToken);
    }
}