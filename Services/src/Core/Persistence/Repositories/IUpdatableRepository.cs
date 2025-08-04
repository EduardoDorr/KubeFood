using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface IUpdatableRepository<in TEntity, TId> where TEntity : BaseEntity<TId>
{
    void Update(TEntity entity);
}