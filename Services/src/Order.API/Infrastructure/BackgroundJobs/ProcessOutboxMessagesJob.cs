using KubeFood.Core.DomainEvents;
using KubeFood.Core.Persistence.Outbox;
using KubeFood.Order.API.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace KubeFood.Order.API.Infrastructure.BackgroundJobs;

internal sealed class ProcessOutboxMessagesJob : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(IServiceProvider provider, ILogger<ProcessOutboxMessagesJob> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            var outboxMessages = await context
                .Set<OutboxMessage>()
                .Where(om => !om.Processed)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var outboxMessage in outboxMessages)
            {
                var domainEvent =
                    JsonConvert.DeserializeObject<IDomainEvent>(
                        outboxMessage.Content,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                        });

                if (domainEvent is null)
                {
                    _logger.LogError("Domain event {OutboxMessageType} cannot deserialize", outboxMessage.Type);

                    continue;
                }

                await publisher.Publish(domainEvent, stoppingToken);

                outboxMessage.Processed = true;
                outboxMessage.ProcessedAt = DateTime.Now;
            }

            await context.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}