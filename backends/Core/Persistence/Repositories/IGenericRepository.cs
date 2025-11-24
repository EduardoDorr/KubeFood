using KubeFood.Core.Entities;

namespace KubeFood.Core.Persistence.Repositories;

public interface IGenericRepository<TEntity, TId>
    : IReadableRepository<TEntity, TId>,
      ICreatableRepository<TEntity, TId>,
      IUpdatableRepository<TEntity, TId>,
      IDeletableRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{ }