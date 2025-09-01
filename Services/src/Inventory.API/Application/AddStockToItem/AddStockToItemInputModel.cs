namespace KubeFood.Inventory.API.Application.AddStockToItem;

public sealed record AddStockToItemInputModel(
    int Quantity);

public static class AddStockToItemInputModelExtensions
{
    public static AddStocktoItemCommand ToCommand(this AddStockToItemInputModel model, int id)
        => new(
            id,
            model.Quantity);
}