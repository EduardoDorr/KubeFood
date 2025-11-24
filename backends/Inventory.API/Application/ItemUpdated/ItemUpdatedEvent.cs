using KubeFood.Core.Events;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.ItemUpdated;

public sealed record ItemUpdatedEvent(
    string Id,
    string Name,
    InventoryItemCategory Category,
    decimal Weight,
    bool IsActive = true)
    : IDomainEvent;