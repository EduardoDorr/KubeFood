using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.CreateItem;

public sealed record CreateItemCommand(
    string ProductId,
    string Name,
    InventoryItemCategory Category,
    int InitialQuantity = 0);

public static class CreateItemCommandExtensions
{
    public static InventoryItem ToItem(this CreateItemCommand command)
        => new(
            command.ProductId,
            command.Name,
            command.Category,
            command.InitialQuantity);
}