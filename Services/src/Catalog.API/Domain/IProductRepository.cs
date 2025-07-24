using Core.Models.Pagination;

using MongoDB.Bson;

namespace Catalog.API.Domain;

public interface IProductRepository
{
    Task<PaginationResult<Product>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<ObjectId> productIds, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);
    void Create(Product product);
    void Update(Product product);
    void Delete(Product product);
}