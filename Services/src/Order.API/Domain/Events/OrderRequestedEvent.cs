using KubeFood.Core.Events;

namespace KubeFood.Order.API.Domain.Events;

public sealed record OrderRequestedEvent(
    Guid Id,
    List<OrderRequestedItemEvent> Items)
    : IDomainEvent;

public sealed record OrderRequestedItemEvent(
    string Id,
    int Quantity);