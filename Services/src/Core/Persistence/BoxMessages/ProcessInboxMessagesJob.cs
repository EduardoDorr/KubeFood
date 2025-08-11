using KubeFood.Core.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class ProcessInboxMessagesJob<TId, TContext> : BackgroundService
    where TContext : DbContext
{
    private const int BATCH_SIZE = 20;
    private const int PROCESS_INTERVAL_SECONDS = 60;

    private readonly IServiceProvider _provider;
    private readonly ILogger<ProcessInboxMessagesJob<TId, TContext>> _logger;

    public ProcessInboxMessagesJob(IServiceProvider provider, ILogger<ProcessInboxMessagesJob<TId, TContext>> logger)
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
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

                var inboxMessages = await context
                    .Set<InboxMessage<TId>>()
                    .Where(om => !om.Processed)
                    .Take(BATCH_SIZE)
                    .ToListAsync(stoppingToken);

                foreach (var inboxMessage in inboxMessages)
                {
                    try
                    {
                        var @event =
                            JsonConvert.DeserializeObject<IEvent>(
                                inboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All,
                                });

                        if (@event is null)
                        {
                            inboxMessage.Processed = true;
                            inboxMessage.ProcessedAt = DateTime.Now;

                            _logger.LogError("Domain event {OutboxMessageType} cannot deserialize", inboxMessage.Type);

                            continue;
                        }

                        await dispatcher.DispatchAsync(@event, stoppingToken);

                        inboxMessage.Processed = true;
                        inboxMessage.ProcessedAt = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        inboxMessage.RetryCount++;
                        _logger.LogError(ex, "Error processing outbox message {EventId} | retry: {Retries}", inboxMessage.Id, inboxMessage.RetryCount);

                        if (inboxMessage.RetryCount > 5)
                        {
                            _logger.LogWarning("Max retries reached for event {EventId}, marking as processed for no new attempts", inboxMessage.Id);
                            inboxMessage.Error = ex.Message;
                            inboxMessage.Processed = true;
                            inboxMessage.ProcessedAt = DateTime.Now;
                        }
                    }
                }

                if (inboxMessages.Count > 0)
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