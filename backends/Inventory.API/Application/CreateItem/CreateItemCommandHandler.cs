using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.CreateItem;

public interface ICreateItemCommandHandler
    : IRequestHandler<CreateItemCommand, Result<string>>
{ }

public class CreateItemCommandHandler : ICreateItemCommandHandler
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateItemCommandHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateItemCommand request, CancellationToken cancellationToken = default)
    {
        var item = request.ToItem();

        _inventoryRepository.Create(item);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result < 1)
            return Result.Fail<string>(InventoryItemError.CannotCreate);

        return Result.Ok(item.ProductId);
    }
}