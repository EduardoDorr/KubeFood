using KubeFood.Core.DomainEvents;

namespace KubeFood.Order.API.Domain;

public sealed record OrderRequested(
    Guid OrderUniqueId,
    List<string> OrderItemIds)
    : IDomainEvent;