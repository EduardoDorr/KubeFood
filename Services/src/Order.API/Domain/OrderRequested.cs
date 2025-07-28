using KubeFood.Core.DomainEvents;

namespace KubeFood.Order.API.Domain;

public sealed record OrderRequested(
    int OrderId,
    List<string> OrderItemIds)
    : IDomainEvent;