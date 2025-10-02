using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.UpdateItem;

public interface IUpdateItemCommandHandler
    : IRequestHandler<UpdateItemCommand, Result>
{ }

public class UpdateItemCommandHandler : IUpdateItemCommandHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItemCommandHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken = default)
    {
        var item = await _inventoryRepository
            .GetByIdAsync(
                request.Id,
                cancellationToken);

        if (item is null)
            return Result.Fail(InventoryItemError.NotFound);

        item.Update(
            request.Name,
            request.Category,
            request.IsActive ?? true);

        _inventoryRepository.Update(item);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result < 1)
            return Result.Fail(InventoryItemError.CannotUpdate);

        return Result.Ok();
    }
}