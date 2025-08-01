using KubeFood.Core.ValueObjects;

namespace KubeFood.Order.API.Application.UpdateOrder;

public sealed record UpdateOrderInputModel(
    Address DeliveryAddress);

public static class UpdateProductInputModelExtensions
{
    public static UpdateOrderCommand ToCommand(this UpdateOrderInputModel model, int id)
        => new(
            id,
            model.DeliveryAddress);
}