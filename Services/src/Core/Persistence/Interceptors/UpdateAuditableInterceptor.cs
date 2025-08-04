using KubeFood.Core.Entities.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KubeFood.Core.Persistence.Interceptors;

public sealed class UpdateAuditableInterceptor<TId> : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        UpdateUpdatedAtAsync(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateUpdatedAtAsync(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IAuditable<TId>>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdatedAtDate(DateTime.Now);
        }
    }
}