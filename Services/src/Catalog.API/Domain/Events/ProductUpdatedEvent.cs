using KubeFood.Core.Events;

namespace KubeFood.Catalog.API.Domain.Events;

public sealed record ProductUpdatedEvent(
    string Id,
    string Name,
    ProductCategory Category,
    decimal Weight,
    bool IsActive = true)
    : IDomainEvent;