
using Catalog.API.Domain;

using Core.Models.Pagination;

using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Persistence.Repositories;

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

    public async Task<Product?> GetByIdAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Uuid == uuid, cancellationToken);
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