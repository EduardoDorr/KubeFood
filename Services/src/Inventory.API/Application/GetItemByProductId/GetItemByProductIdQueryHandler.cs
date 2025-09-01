using KubeFood.Core.Interfaces;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Application.Models;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.GetItemByProductId;

public interface IGetItemByProductIdQueryHandler
    : IRequestHandler<GetItemByProductIdQuery, Result<ItemViewModel?>>
{ }

public class GetItemByProductIdQueryHandler : IGetItemByProductIdQueryHandler
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetItemByProductIdQueryHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<ItemViewModel?>> HandleAsync(GetItemByProductIdQuery request, CancellationToken cancellationToken = default)
    {
        var item =
            await _inventoryRepository
                .GetByProductIdAsync(
                    request.Id,
                    cancellationToken);

        if (item is null)
            return Result.Fail<ItemViewModel?>(InventoryItemError.NotFound);

        return Result.Ok(item.ToModel());
    }
}