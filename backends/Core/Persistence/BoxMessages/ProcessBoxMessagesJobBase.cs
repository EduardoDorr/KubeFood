using KubeFood.Core.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace KubeFood.Core.Persistence.BoxMessages;

public abstract class ProcessBoxMessagesJobBase<TEntity, TId, TOptions, TDbContext> : BackgroundService
    where TEntity : BaseBoxMessage<TId>
    where TOptions : BaseBoxMessageOptions
    where TDbContext : DbContext
{
    private const int RETRY_LIMIT = 5;
    private const int BATCH_SIZE = 20;
    private const int PROCESS_INTERVAL_IN_SECONDS = 60;

    private readonly IServiceProvider _provider;
    private readonly TOptions? _options;
    private readonly ILogger _logger;

    protected ProcessBoxMessagesJobBase(
        IServiceProvider provider,
        ILogger logger,
        IOptions<TOptions>? options = null)
    {
        _provider = provider;
        _options = options?.Value;
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

                var batchSize = _options?.BatchSize ?? BATCH_SIZE;
                var messages = await context.Set<TEntity>()
                    .Where(m => !m.Processed)
                    .Take(batchSize)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var @event =
                            JsonConvert.DeserializeObject<IEvent>(
                                message.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All,
                                });

                        if (@event is null)
                        {
                            MarkProcessed(message, "Cannot deserialize event");
                            continue;
                        }

                        await dispatcher.DispatchAsync(@event, stoppingToken);
                        MarkProcessed(message);
                    }
                    catch (Exception ex)
                    {
                        message.RetryCount++;
                        _logger.LogError(ex, "Error processing message {EventId} | retry {Retries}", message.Id, message.RetryCount);

                        var retryLimit = _options?.RetryLimit ?? RETRY_LIMIT;
                        if (message.RetryCount > retryLimit)
                        {
                            _logger.LogWarning("Max retries reached for event {EventId}, marking as processed for no new attempts", message.Id);
                            MarkProcessed(message, ex.Message);
                        }
                    }
                }

                if (messages.Count > 0)
                    await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in {BackgroundJob}", GetType().Name);
            }
            finally
            {
                var processIntervalInSeconds = _options?.ProcessIntervalInSeconds ?? PROCESS_INTERVAL_IN_SECONDS;
                await Task.Delay(TimeSpan.FromSeconds(processIntervalInSeconds), stoppingToken);
            }
        }
    }

    private static void MarkProcessed(TEntity message, string? error = null)
    {
        message.Processed = true;
        message.ProcessedAt = DateTime.Now;

        if (!string.IsNullOrEmpty(error))
            message.Error = error;
    }
}