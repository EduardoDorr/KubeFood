using KubeFood.Core.DomainEvents;

namespace KubeFood.Order.API.Domain;

public sealed record OrderRequested(
    Guid OrderUniqueId,
    List<OrderRequestedItem> OrderItems)
    : IDomainEvent;

public sealed record OrderRequestedItem(
    string Id,
    int Quantity);