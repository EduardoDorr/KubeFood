using KubeFood.Core.Models.Pagination;

namespace KubeFood.Inventory.API.Domain;

public interface IInventoryRepository
{
    Task<PaginationResult<InventoryItem>> GetAllAsync(string? productName = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default);
    Task<bool> IsUniqueAsync(string productId, CancellationToken cancellationToken = default);
    void Create(InventoryItem item);
    void Update(InventoryItem item);
}