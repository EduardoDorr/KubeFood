using KubeFood.Core.Events;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.ItemCreated;

public sealed record ItemCreatedEvent(
    string Id,
    string Name,
    InventoryItemCategory Category,
    decimal Weight)
    : IDomainEvent;