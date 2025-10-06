using KubeFood.Core.Events;

namespace KubeFood.Inventory.API.Application.StockReservationConsumed;

public sealed record StockReservationConsumedEvent(
    Guid Id,
    List<StockReservationConsumedItemEvent> Items)
    : IDomainEvent;

public sealed record StockReservationConsumedItemEvent(
    string Id,
    int Quantity);