using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;
using KubeFood.Core.MessageBus;

namespace KubeFood.Catalog.API.Infrastructure.Consumer;

public sealed record OrderRequested(Guid OrderUniqueId, List<OrderRequestedItem> OrderItems);
public sealed record OrderRequestedItem(string Id, int Quantity);

public sealed record OrderValidated(Guid OrderUniqueId, List<OrderValidatedItem> OrderItems);
public sealed record OrderValidatedItem(string ProductId, string Name, decimal Price, int Quantity);

public class OrderRequestedConsumer : IMessageBusConsumerService<OrderRequested>
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBusProducerService _messageBusProducerService;

    public OrderRequestedConsumer(
        IProductRepository productRepository,
        IMessageBusProducerService messageBusProducerService)
    {
        _productRepository = productRepository;
        _messageBusProducerService = messageBusProducerService;
    }

    public async Task ConsumeAsync(OrderRequested? message, CancellationToken cancellationToken = default)
    {
        if (message is null)
            return;

        var objectIds = message?.OrderItems.Select(o => o.Id.DecodeObjectId());

        var products = await _productRepository
            .GetByIdsAsync(objectIds, cancellationToken);

        var orderItems = products.ToOrderItem(message.OrderItems);
        var orderValidated = new OrderValidated(message.OrderUniqueId, orderItems);

        await _messageBusProducerService.PublishAsync(nameof(OrderValidated), orderValidated, cancellationToken);
    }
}

public static class OrderValidatedItemExtensions
{
    public static OrderValidatedItem ToOrderItem(this Product product, int quantity)
        => new(
            product.Id.EncodeId(),
            product.Name,
            product.Value,
            quantity);

    public static List<OrderValidatedItem> ToOrderItem(this IEnumerable<Product> products, IEnumerable<OrderRequestedItem> requestedItems)
        => products is null
            ? []
            : products.Select(p =>
                p.ToOrderItem(
                    requestedItems
                        .SingleOrDefault(r => r.Id == p.Uuid)!.Quantity)).ToList();
}