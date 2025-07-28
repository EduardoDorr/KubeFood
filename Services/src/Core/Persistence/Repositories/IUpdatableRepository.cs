using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface IUpdatableRepository<in TEntity> where TEntity : BaseEntity
{
    void Update(TEntity entity);
}