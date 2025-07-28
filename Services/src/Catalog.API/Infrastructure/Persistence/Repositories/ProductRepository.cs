using KubeFood.Catalog.API.Domain;
using KubeFood.Catalog.API.Infrastructure.Persistence;
using KubeFood.Core.Models.Pagination;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;
using MongoDB.Driver;

namespace KubeFood.Catalog.API.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<Product>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var products = _context.Products.AsNoTracking();

        return await products
            .GetPaged(page, pageSize, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<ObjectId> productIds, CancellationToken cancellationToken = default)
    {
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        return products;
    }

    public async Task<Product?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Create(Product entity)
    {
        _context.Products.Add(entity);
    }

    public void Update(Product entity)
    {
        _context.Products.Update(entity);
    }

    public void Delete(Product entity)
    {
        _context.Products.Remove(entity);
    }
}