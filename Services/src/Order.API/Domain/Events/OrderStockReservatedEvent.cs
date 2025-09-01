using KubeFood.Core.Events;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderStockReservatedEvent(
    Guid OrderUniqueId,
    bool IsStockReservated,
    List<string>? UnavailableItems = null)
    : IDomainEvent;