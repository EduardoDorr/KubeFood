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
        var objectIds = message.OrderItems.Select(o => o.Id.DecodeObjectId());

        if (objectIds is null)
        {
            var orderInvalid =
                new OrderValidatedEvent(
                    message!.OrderUniqueId,
                    false,
                    [],
                    message.OrderItems.Select(o => o.Id).ToList());

            await _messageBusProducerService
                .PublishAsync(nameof(OrderValidatedEvent), orderInvalid, cancellationToken);

            return MessageBusConsumerResult.Ack;
        }

        var products = await _productRepository
            .GetByIdsAsync(objectIds!, cancellationToken);

        if (objectIds!.Count() > products.Count())
        {
            var orderInvalid =
                new OrderValidatedEvent(
                    message!.OrderUniqueId,
                    false,
                    [],
                    message.OrderItems.Select(o => o.Id).ToList());

            await _messageBusProducerService
                .PublishAsync(nameof(OrderValidatedEvent), orderInvalid, cancellationToken);
        }

        var orderItems = products.ToOrderItem(message.OrderItems);
        var orderValidated =
            new OrderValidatedEvent(
                message.OrderUniqueId,
                true,
                orderItems);

        await _messageBusProducerService
            .PublishAsync(nameof(OrderValidatedEvent), orderValidated, cancellationToken);

        return MessageBusConsumerResult.Ack;
    }
}