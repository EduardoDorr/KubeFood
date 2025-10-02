using KubeFood.Core.Events;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderStockReservationRequestedEvent(
    Guid Id,
    List<OrderStockReservationRequestedItemEvent> Items)
    : IDomainEvent;

public sealed record OrderStockReservationRequestedItemEvent(
    string Id,
    int Quantity);