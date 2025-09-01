namespace KubeFood.Inventory.API.Application.AddStockToItem;

public sealed record AddStocktoItemCommand(
    int Id,
    int Quantity);