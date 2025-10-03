using KubeFood.Core.Events;

namespace KubeFood.Order.API.Application.OrderOrchestration.OrderStockReserved;

public sealed record OrderStockReservedEvent(
    Guid Id,
    bool IsStockReservated,
    List<string>? UnavailableItems = null)
    : IDomainEvent;