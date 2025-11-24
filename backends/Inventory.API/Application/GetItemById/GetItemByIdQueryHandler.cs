using KubeFood.Core.Interfaces;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Application.Models;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.GetItemById;

public interface IGetItemByIdQueryHandler
    : IRequestHandler<GetItemByIdQuery, Result<ItemViewModel?>>
{ }

public class GetItemByIdQueryHandler : IGetItemByIdQueryHandler
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetItemByIdQueryHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<ItemViewModel?>> HandleAsync(GetItemByIdQuery request, CancellationToken cancellationToken = default)
    {
        var item =
            await _inventoryRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (item is null)
            return Result.Fail<ItemViewModel?>(InventoryItemError.NotFound);

        return Result.Ok(item.ToModel());
    }
}