using KubeFood.Core.Entities;

namespace KubeFood.Inventory.API.Domain;

public class InventoryItemMovement : BaseEntity<int>
{
    public int InventoryId { get; private set; }
    public int Quantity { get; private set; }
    public InventoryMovementType Type { get; private set; }
    public Guid? OrderId { get; private set; }

    public virtual InventoryItem InventoryItem { get; private set; }

    private InventoryItemMovement() { }

    public InventoryItemMovement(int inventoryId, int quantity, InventoryMovementType type, Guid? orderId = null)
    {
        InventoryId = inventoryId;
        Quantity = quantity;
        Type = type;
        OrderId = orderId;
    }
}