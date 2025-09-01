using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.CreateItem;

public sealed record CreateItemCommand(
    string ProductId,
    string ProductName,
    int InitialQuantity);

public static class CreateItemCommandExtensions
{
    public static InventoryItem ToItem(this CreateItemCommand command)
        => new(
            command.ProductId,
            command.ProductName,
            command.InitialQuantity);
}