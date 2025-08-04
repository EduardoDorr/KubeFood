using System.Linq.Expressions;

using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface IReadableByRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> GetSingleByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}