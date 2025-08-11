using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Events;
using KubeFood.Core.Helpers;
using KubeFood.Core.MessageBus;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Application.ValidateProducts;

public class OrderRequestedEventHandler : EventHandlerBase<OrderRequestedEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderRequestedEventHandler(
        ILogger<OrderRequestedEventHandler> logger,
        IProductRepository productRepository,
        IMessageBusProducerService messageBusProducerService)
        : base(logger)
    {
        _productRepository = productRepository;
        _messageBusProducerService = messageBusProducerService;
    }

    protected override async Task ExecuteAsync(OrderRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        OrderValidatedEvent orderValidatedEvent;

        var objectIds = @event
            .OrderItems
            .Select(o => o.Id.DecodeHashId<ObjectId>());

        if (objectIds is null)
        {
            orderValidatedEvent =
                new OrderValidatedEvent(
                    @event!.OrderUniqueId,
                    false,
                    [],
                    @event.OrderItems.Select(o => o.Id).ToList());
        }
        else
        {
            var products = await _productRepository
                .GetByIdsAsync(objectIds!, cancellationToken);

            var orderItems = products
                .ToOrderItem(@event.OrderItems);

            if (objectIds!.Count() > products.Count())
            {
                var invalidItems = @event.OrderItems
                    .Where(oi => products.All(p => p.Id != oi.Id.DecodeHashId<ObjectId>()))
                    .ToList();

                orderValidatedEvent =
                    new OrderValidatedEvent(
                        @event!.OrderUniqueId,
                        false,
                        orderItems,
                        invalidItems.Select(i => i.Id).ToList());
            }
            else
            {
                orderValidatedEvent =
                    new OrderValidatedEvent(
                        @event.OrderUniqueId,
                        true,
                        orderItems);
            }
        }

        await _messageBusProducerService
            .PublishAsync(nameof(OrderValidatedEvent), orderValidatedEvent, cancellationToken);
    }
}