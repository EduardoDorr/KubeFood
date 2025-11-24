using KubeFood.Core.Events;
using KubeFood.Order.API.Domain;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderValidatedEvent;

public sealed record OrderValidatedEvent(
    Guid Id,
    bool Valid,
    List<OrderValidatedItemEvent> Items,
    List<string>? InvalidItems = null)
    : IEvent;

public sealed record OrderValidatedItemEvent(
    string Id,
    string Name,
    decimal Price,
    int Quantity);

public static class OrderValidatedItemEventExtensions
{
    public static OrderItem ToOrderItem(this OrderValidatedItemEvent itemValidated, int orderId)
        => new(
            orderId,
            itemValidated.Id,
            itemValidated.Name,
            itemValidated.Price,
            itemValidated.Quantity);

    public static IEnumerable<OrderItem> ToOrderItem(this IEnumerable<OrderValidatedItemEvent> itemValidated, int orderId)
        => itemValidated is null
            ? []
            : itemValidated.Select(iv => iv.ToOrderItem(orderId));
}