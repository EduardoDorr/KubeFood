using KubeFood.Core.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class ProcessOutboxMessagesJob<TId, TDbContext> : BackgroundService
    where TDbContext : DbContext
{
    private const int BATCH_SIZE = 20;
    private const int PROCESS_INTERVAL_SECONDS = 60;

    private readonly IServiceProvider _provider;
    private readonly ILogger<ProcessOutboxMessagesJob<TId, TDbContext>> _logger;

    public ProcessOutboxMessagesJob(IServiceProvider provider, ILogger<ProcessOutboxMessagesJob<TId, TDbContext>> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

                var outboxMessages = await context
                    .Set<OutboxMessage<TId>>()
                    .Where(om => !om.Processed)
                    .Take(BATCH_SIZE)
                    .ToListAsync(stoppingToken);

                foreach (var outboxMessage in outboxMessages)
                {
                    try
                    {
                        var domainEvent =
                            JsonConvert.DeserializeObject<IEvent>(
                                outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All,
                                });

                        if (domainEvent is null)
                        {
                            outboxMessage.Processed = true;
                            outboxMessage.ProcessedAt = DateTime.Now;

                            _logger.LogError("Domain event {OutboxMessageType} cannot deserialize", outboxMessage.Type);

                            continue;
                        }

                        await dispatcher.DispatchAsync(domainEvent, stoppingToken);

                        outboxMessage.Processed = true;
                        outboxMessage.ProcessedAt = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        outboxMessage.RetryCount++;
                        _logger.LogError(ex, "Error processing outbox message {EventId} | retry: {Retries}", outboxMessage.Id, outboxMessage.RetryCount);

                        if (outboxMessage.RetryCount > 5)
                        {
                            _logger.LogWarning("Max retries reached for event {EventId}, marking as processed for no new attempts", outboxMessage.Id);
                            outboxMessage.Error = ex.Message;
                            outboxMessage.Processed = true;
                            outboxMessage.ProcessedAt = DateTime.Now;
                        }
                    }
                }

                if (outboxMessages.Count > 0)
                    await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in {BackgroundJob}", GetType().Name);
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(PROCESS_INTERVAL_SECONDS), stoppingToken);
            }
        }
    }
}