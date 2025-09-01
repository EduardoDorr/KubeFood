using KubeFood.Core.Results.Errors;

namespace KubeFood.Inventory.API.Domain;

public static class InventoryItemError
{
    public static Error NotFound =>
        new("InventoryItemError.NotFound", "Could not find a product with this productId", ErrorType.NotFound);

    public static Error InvalidQuantity =>
        new("InventoryItemError.InvalidQuantity", "Quantity must be greater than 0", ErrorType.Validation);

    public static Error InsufficientStock =>
        new("InventoryItemError.InsufficientStock", "There aren't enough quantity available in stock", ErrorType.Validation);

    public static Error InsufficientReservation =>
        new("InventoryItemError.InsufficientReservation", "There aren't enough quantity available in reservation", ErrorType.Validation);

    public static Error CannotCreate =>
        new("InventoryItemError.CannotCreate", "Something prevented from creating a new inventory item", ErrorType.Failure);

    public static Error CannotUpdate =>
        new("InventoryItemError.CannotUpdate", "Something prevented from updating an inventory item", ErrorType.Failure);

    public static Error CannotDelete =>
        new("InventoryItemError.CannotDelete", "Something prevented from deleting an inventory item", ErrorType.Failure);
}