﻿using Core.Entities;

namespace Core.Persistence.Repositories;

public interface IDeletableRepository<in TEntity> where TEntity : BaseEntity
{
    void Delete(TEntity entity);
}