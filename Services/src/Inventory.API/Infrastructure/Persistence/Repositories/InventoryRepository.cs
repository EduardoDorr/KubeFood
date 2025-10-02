using KubeFood.Core.Models.Pagination;
using KubeFood.Inventory.API.Domain;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Inventory.API.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;

    public InventoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<InventoryItem>> GetAllAsync(string? productName = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var items = _context.Inventory.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(productName))
            items = items.Where(i => i.Name == productName);

        return await items
            .GetPagedAsync(page, pageSize, cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .Include(o => o.Movements)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<InventoryItem?> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .Include(o => o.Movements)
            .SingleOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
    }

    public async Task<bool> IsUniqueAsync(string productId, CancellationToken cancellationToken = default)
    {
        var hasAny = await _context.Inventory
            .AnyAsync(p => p.ProductId == productId, cancellationToken);

        return !hasAny;
    }

    public void Create(InventoryItem item)
    {
        _context.Inventory.Add(item);
    }

    public void Update(InventoryItem item)
    {
        _context.Inventory.Update(item);
    }
}