using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface ICreatableRepository<in TEntity> where TEntity : BaseEntity
{
    void Create(TEntity entity);
}