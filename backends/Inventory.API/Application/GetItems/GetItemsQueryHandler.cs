using KubeFood.Core.Interfaces;
using KubeFood.Core.Models.Pagination;
using KubeFood.Core.Results.Base;
using KubeFood.Inventory.API.Application.Models;
using KubeFood.Inventory.API.Domain;

namespace KubeFood.Inventory.API.Application.GetItems;

public interface IGetItemsQueryHandler
    : IRequestHandler<GetItemsQuery, Result<PaginationResult<ItemViewModel>>>
{ }

public class GetItemsQueryHandler : IGetItemsQueryHandler
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetItemsQueryHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<PaginationResult<ItemViewModel>>> HandleAsync(GetItemsQuery request, CancellationToken cancellationToken = default)
    {
        var items =
            await _inventoryRepository
                .GetAllAsync(
                    request.ProductName,
                    request.Pagination.Page,
                    request.Pagination.PageSize,
                    cancellationToken);

        var paginatedItems =
            items
                .Map(items.Data.ToModel().ToList());

        return Result.Ok(paginatedItems);
    }
}