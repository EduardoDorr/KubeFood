using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.DeleteItem;

public interface IDeleteItemCommandHandler
    : IRequestHandler<DeleteItemCommand, Result>
{ }

public class DeleteItemCommandHandler : IDeleteItemCommandHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemCommandHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken = default)
    {
        var item =
            await _inventoryRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (item is null)
            return Result.Fail(InventoryItemError.NotFound);

        item.Deactivate();

        _inventoryRepository.Update(item);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
            return Result.Fail(InventoryItemError.CannotDelete);

        return Result.Ok();
    }
}