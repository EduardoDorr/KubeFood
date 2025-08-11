using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;

namespace KubeFood.Catalog.API.Application.ValidateProducts;

public sealed record OrderValidatedEvent(Guid OrderUniqueId, bool Valid, List<OrderValidatedItemEvent> OrderItems, List<string>? InvalidOrderItems = null);
public sealed record OrderValidatedItemEvent(string ProductId, string Name, decimal Price, int Quantity);

public static class OrderValidatedItemExtensions
{
    public static OrderValidatedItemEvent ToOrderItem(this Product product, int quantity)
        => new(
            product.Id.EncodeId(),
            product.Name,
            product.Value,
            quantity);

    public static List<OrderValidatedItemEvent> ToOrderItem(this IEnumerable<Product> products, IEnumerable<OrderRequestedItemEvent> requestedItems)
        => products is null
            ? []
            : products.Select(p =>
                p.ToOrderItem(
                    requestedItems
                        .SingleOrDefault(r => r.Id == p.Id.EncodeId())!.Quantity)).ToList();
}