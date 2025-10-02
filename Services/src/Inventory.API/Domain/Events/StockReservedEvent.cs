using KubeFood.Core.Events;

namespace KubeFood.Inventory.API.Domain.Events;

public sealed record StockReservedEvent(
    Guid Id,
    bool IsStockReservated,
    List<string>? UnavailableItems = null)
    : IDomainEvent;