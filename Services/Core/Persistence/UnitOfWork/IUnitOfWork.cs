﻿namespace Core.Persistence.UnitOfWork;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}