using Core.Entities;
using Core.Models.Pagination;

namespace Core.Persistence.Repositories;

public interface IReadableRepository<TEntity> where TEntity : BaseEntity
{
    Task<PaginationResult<TEntity>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}