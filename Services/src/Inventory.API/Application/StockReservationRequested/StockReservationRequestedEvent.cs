using KubeFood.Core.Events;

namespace KubeFood.Inventory.API.Application.StockReservationRequested;

public sealed record StockReservationRequestedEvent(
    Guid Id,
    List<StockReservationRequestedItemEvent> Items)
    : IDomainEvent;

public sealed record StockReservationRequestedItemEvent(
    string Id,
    int Quantity);