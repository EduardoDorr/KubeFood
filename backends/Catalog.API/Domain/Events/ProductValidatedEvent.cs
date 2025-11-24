using KubeFood.Catalog.API.Application.ProductValidationRequested;
using KubeFood.Core.Events;
using KubeFood.Core.Helpers;

namespace KubeFood.Catalog.API.Domain.Events;

public sealed record ProductValidatedEvent(
    Guid Id,
    bool Valid,
    List<ProductValidatedItemEvent> Items,
    List<string>? InvalidItems = null) : IEvent;
public sealed record ProductValidatedItemEvent(
    string Id,
    string Name,
    decimal Price,
    int Quantity);

public static class ProductValidatedItemExtensions
{
    public static ProductValidatedItemEvent ToOrderItem(this Product product, int quantity)
        => new(
            product.Id.EncodeId(),
            product.Name,
            product.Value,
            quantity);

    public static List<ProductValidatedItemEvent> ToOrderItem(this IEnumerable<Product> products, IEnumerable<ProductValidationRequestedItemEvent> requestedItems)
        => products is null
            ? []
            : products.Select(p =>
                p.ToOrderItem(
                    requestedItems
                        .SingleOrDefault(r => r.Id == p.Id.EncodeId())!.Quantity)).ToList();
}