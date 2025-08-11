using KubeFood.Core.Events;

namespace KubeFood.Catalog.API.Domain.Events;

public sealed record ProductCreatedEvent(
    string Id,
    string Name,
    ProductCategory Category,
    decimal Weight)
    : IDomainEvent;