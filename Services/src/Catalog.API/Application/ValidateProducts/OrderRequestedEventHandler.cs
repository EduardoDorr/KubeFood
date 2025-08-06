using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;
using KubeFood.Core.MessageBus;

namespace KubeFood.Catalog.API.Application.ValidateProducts;

public class OrderRequestedEventHandler : MessageBusConsumerServiceHandlerBase<OrderRequestedEvent>
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

    protected async override Task<MessageBusConsumerResult> ExecuteAsync(OrderRequestedEvent message, CancellationToken cancellationToken = default)
    {
        OrderValidatedEvent orderValidatedEvent;

        var objectIds = message.OrderItems.Select(o => o.Id.DecodeObjectId());

        if (objectIds is null)
        {
            orderValidatedEvent =
                new OrderValidatedEvent(
                    message!.OrderUniqueId,
                    false,
                    [],
                    message.OrderItems.Select(o => o.Id).ToList());
        }
        else
        {
            var products = await _productRepository
                .GetByIdsAsync(objectIds!, cancellationToken);

            var orderItems = products
                .ToOrderItem(message.OrderItems);

            if (objectIds!.Count() > products.Count())
            {
                var invalidItems = message.OrderItems
                    .Where(oi => products.All(p => p.Id != oi.Id.DecodeObjectId()))
                    .ToList();

                orderValidatedEvent =
                    new OrderValidatedEvent(
                        message!.OrderUniqueId,
                        false,
                        orderItems,
                        invalidItems.Select(i => i.Id).ToList());
            }
            else
            {
                orderValidatedEvent =
                    new OrderValidatedEvent(
                        message.OrderUniqueId,
                        true,
                        orderItems);
            }
        }

        await _messageBusProducerService
            .PublishAsync(nameof(OrderValidatedEvent), orderValidatedEvent, cancellationToken);

        return MessageBusConsumerResult.Ack;
    }
}