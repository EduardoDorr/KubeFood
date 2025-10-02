using KubeFood.Core.Entities;
using KubeFood.Core.Results.Base;
using KubeFood.Core.Results.Extensions;

namespace KubeFood.Inventory.API.Domain;

public class InventoryItem : BaseEntity<int>
{
    public string ProductId { get; private set; }
    public string Name { get; private set; }
    public InventoryItemCategory Category { get; private set; }
    public int QuantityAvailable { get; private set; }
    public int QuantityReserved { get; private set; }

    public virtual ICollection<InventoryItemMovement> Movements { get; private set; } = [];

    private InventoryItem() { }

    public InventoryItem(string productId, string name, InventoryItemCategory category, int initialQuantity = 0)
    {
        ProductId = productId;
        Name = name;
        Category = category;
        QuantityAvailable = initialQuantity;
        QuantityReserved = 0;
    }

    public Result Update(string name, InventoryItemCategory category, bool active = true)
    {
        Name = name;
        Category = category;
        IsActive = active;

        return Result.Ok();
    }

    public Result AddStock(int quantity, Guid? orderId = null)
    {
        var validation = ValidateQuantity(quantity);

        if (!validation.Success)
            return validation;

        QuantityAvailable += quantity;
        Movements.Add(new(Id, quantity, InventoryMovementType.Add, orderId));

        return Result.Ok();
    }


    public Result RemoveStock(int quantity, Guid? orderId = null)
    {
        var validation = ValidateQuantityAndAvailability(quantity);

        if (!validation.Success)
            return validation;

        QuantityAvailable -= quantity;
        Movements.Add(new(Id, quantity, InventoryMovementType.Remove, orderId));

        return Result.Ok();
    }

    public Result ReserveStock(int quantity, Guid? orderId = null)
    {
        var validation = ValidateQuantityAndAvailability(quantity);

        if (!validation.Success)
            return validation;

        QuantityAvailable -= quantity;
        QuantityReserved += quantity;
        Movements.Add(new(Id, quantity, InventoryMovementType.Reserve, orderId));

        return Result.Ok();
    }

    public Result ReleaseReservedStock(int quantity, Guid? orderId = null)
    {
        var validation = ValidateQuantityAndReservation(quantity);

        if (!validation.Success)
            return validation;

        QuantityReserved -= quantity;
        QuantityAvailable += quantity;
        Movements.Add(new(Id, quantity, InventoryMovementType.ReleaseReserve, orderId));

        return Result.Ok();
    }

    public Result ConfirmReservation(int quantity, Guid? orderId = null)
    {
        var validation = ValidateQuantityAndReservation(quantity);

        if (!validation.Success)
            return validation;

        QuantityReserved -= quantity;
        Movements.Add(new(Id, quantity, InventoryMovementType.ConfirmReserve, orderId));

        return Result.Ok();
    }    

    private static Result ValidateQuantity(int quantity)
    => ResultValidation.ValidateFailFast(
        () => EnsureValidQuantity(quantity));

    private Result ValidateQuantityAndAvailability(int quantity)
        => ResultValidation.ValidateCollectErrors(
            () => EnsureValidQuantity(quantity),
            () => EnsureEnoughAvailableQuantity(quantity));

    private Result ValidateQuantityAndReservation(int quantity)
        => ResultValidation.ValidateCollectErrors(
            () => EnsureValidQuantity(quantity),
            () => EnsureEnoughReservedQuantity(quantity));

    private Result EnsureEnoughAvailableQuantity(int quantity)
        => QuantityAvailable >= quantity
            ? Result.Ok()
            : Result.Fail(InventoryItemError.InsufficientStock);

    private Result EnsureEnoughReservedQuantity(int quantity)
        => QuantityReserved >= quantity
            ? Result.Ok()
            : Result.Fail(InventoryItemError.InsufficientReservation);

    private static Result EnsureValidQuantity(int quantity)
        => quantity > 0
            ? Result.Ok()
            : Result.Fail(InventoryItemError.InvalidQuantity);
}