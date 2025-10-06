using KubeFood.Core.Events;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderStockReservationConsumedEvent(
    Guid Id,
    List<OrderStockReservationConsumedItemEvent> Items)
    : IDomainEvent;

public sealed record OrderStockReservationConsumedItemEvent(
    string Id,
    int Quantity);