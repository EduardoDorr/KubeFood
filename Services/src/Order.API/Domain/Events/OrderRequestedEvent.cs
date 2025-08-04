using KubeFood.Core.DomainEvents;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderRequestedEvent(
    Guid OrderUniqueId,
    List<OrderRequestedItemEvent> OrderItems)
    : IDomainEvent;

public sealed record OrderRequestedItemEvent(
    string Id,
    int Quantity);