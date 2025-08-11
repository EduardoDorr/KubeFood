using KubeFood.Core.Entities;
using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Newtonsoft.Json;

namespace KubeFood.Core.Persistence.Interceptors;

public sealed class PublishDomainEventsToOutBoxMessagesInterceptor<TId> : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        if (eventData.Context is not null)
            await InsertOutBoxMessagesAsync(eventData.Context, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static async Task InsertOutBoxMessagesAsync(DbContext context, CancellationToken cancellationToken)
    {
        var outboxMessages = context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(static domainEvent => new OutboxMessage<TId>
            {
                CreatedAt = DateTime.Now,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    })
            })
            .ToList();

        if (outboxMessages.Count > 0)
            await context.Set<OutboxMessage<TId>>()
                .AddRangeAsync(outboxMessages, cancellationToken);
    }
}