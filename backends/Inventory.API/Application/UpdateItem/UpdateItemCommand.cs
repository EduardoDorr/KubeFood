using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.UpdateItem;

public sealed record UpdateItemCommand(
    int Id,
    string Name,
    InventoryItemCategory Category,
    bool? IsActive = true);