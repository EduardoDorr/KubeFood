using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.AddStockToItem;

public interface IAddStockToItemCommandHandler
    : IRequestHandler<AddStocktoItemCommand, Result>
{ }

public class AddStockToitemCommandHandler : IAddStockToItemCommandHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddStockToitemCommandHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(AddStocktoItemCommand request, CancellationToken cancellationToken = default)
    {
        var item =
            await _inventoryRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (item is null)
            return Result.Fail(InventoryItemError.NotFound);

        var result = item.AddStock(request.Quantity);

        if (!result.Success)
            return result;

        _inventoryRepository.Update(item);
        var updateResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (updateResult <= 0)
            return Result.Fail(InventoryItemError.CannotUpdate);

        return Result.Ok();
    }
}