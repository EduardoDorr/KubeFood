using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.Models;

public sealed record ItemViewModel(
    int Id,
    string ProductId,
    string ProductName,
    int AvailableQuantity);

public static class ProductViewModelExtensions
{
    public static ItemViewModel? ToModel(this InventoryItem item)
        => item is null
            ? null
            : new(
                item.Id,
                item.ProductId,
                item.ProductName,
                item.QuantityAvailable);

    public static IEnumerable<ItemViewModel> ToModel(this IEnumerable<InventoryItem> items)
        => items is null
            ? []
            : items.Select(p => p.ToModel()!);
}