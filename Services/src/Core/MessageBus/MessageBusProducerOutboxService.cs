using KubeFood.Core.Events;
using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace KubeFood.Core.MessageBus;

public sealed class MessageBusProducerOutboxService<TId, TDbContext> : IMessageBusProducerOutboxService
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public MessageBusProducerOutboxService(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string? queue = null, CancellationToken cancellationToken = default)
    {
        var outboxEvent =
            new OutboxEvent(
                queue ?? typeof(TEvent).Name,
                @event!);

        var outboxMessage =
            new OutboxMessage<TId>
            {
                CreatedAt = DateTime.Now,
                Type = outboxEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    outboxEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    })
            };

        _dbContext.Set<OutboxMessage<TId>>()
            .Add(outboxMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}