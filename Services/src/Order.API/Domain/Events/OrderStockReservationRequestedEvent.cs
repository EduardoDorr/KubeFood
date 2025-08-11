using KubeFood.Core.Events;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderStockReservationRequestedEvent(
    Guid OrderUniqueId,
    List<OrderStockReservationRequestedItemEvent> OrderItems)
    : IDomainEvent;

public sealed record OrderStockReservationRequestedItemEvent(
    string Id,
    int Quantity);