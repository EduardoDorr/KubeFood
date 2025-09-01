namespace KubeFood.Inventory.API.Application.RemoveStockFromItem;

public sealed record RemoveStockFromItemInputModel(
    int Quantity);

public static class RemoveStockFromItemInputModelExtensions
{
    public static RemoveStockFromItemCommand ToCommand(this RemoveStockFromItemInputModel model, int id)
        => new(
            id,
            model.Quantity);
}