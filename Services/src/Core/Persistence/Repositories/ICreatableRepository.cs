using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface ICreatableRepository<in TEntity, TId> where TEntity : BaseEntity<TId>
{
    void Create(TEntity entity);
}