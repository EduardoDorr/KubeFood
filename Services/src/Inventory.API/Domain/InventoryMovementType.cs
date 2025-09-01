namespace KubeFood.Inventory.API.Domain;

public enum InventoryMovementType
{
    Add = 0,
    Remove = 1,
    Reserve = 2,
    ReleaseReserve = 3,
    ConfirmReserve = 4
}