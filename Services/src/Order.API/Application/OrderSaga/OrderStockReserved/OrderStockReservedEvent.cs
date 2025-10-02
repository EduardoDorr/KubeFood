using KubeFood.Core.Events;

namespace KubeFood.Order.API.Application.OrderSaga.OrderStockReserved;

public sealed record OrderStockReservedEvent(
    Guid Id,
    bool IsStockReservated,
    List<string>? UnavailableItems = null)
    : IDomainEvent;