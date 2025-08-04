using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface IDeletableRepository<in TEntity, TId> where TEntity : BaseEntity<TId>
{
    void Delete(TEntity entity);
}