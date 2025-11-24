namespace KubeFood.Inventory.API.Application.RemoveStockFromItem;

public sealed record RemoveStockFromItemCommand(
    int Id,
    int Quantity);