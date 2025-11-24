using KubeFood.Core.Entities;
using KubeFood.Core.Models.Pagination;

namespace KubeFood.Core.Persistence.Repositories;

public interface IReadableRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    Task<PaginationResult<TEntity>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}