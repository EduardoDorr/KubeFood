using KubeFood.Core.Events;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.OrderSaga.OrderValidated;

public sealed record OrderValidatedEvent(
    Guid OrderUniqueId,
    bool Valid,
    List<OrderValidatedItemEvent> OrderItems,
    List<string>? InvalidOrderItems = null)
    : IEvent;

public sealed record OrderValidatedItemEvent(
    string ProductId,
    string Name,
    decimal Price,
    int Quantity);

public static class OrderValidatedItemEventExtensions
{
    public static OrderItem ToOrderItem(this OrderValidatedItemEvent itemValidated, int orderId)
        => new(
            orderId,
            itemValidated.ProductId,
            itemValidated.Name,
            itemValidated.Price,
            itemValidated.Quantity);

    public static IEnumerable<OrderItem> ToOrderItem(this IEnumerable<OrderValidatedItemEvent> itemValidated, int orderId)
        => itemValidated is null
            ? []
            : itemValidated.Select(iv => iv.ToOrderItem(orderId));
}