using Microsoft.Extensions.Logging;

namespace KubeFood.Core.Events;

public abstract class EventHandlerBase<T> : IEventHandler<T> where T : IEvent
{
    protected readonly ILogger _logger;

    protected EventHandlerBase(ILogger logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting handling the event type {Type}: {Event}", typeof(T).Name, @event);

            if (@event is null)
                ArgumentNullException.ThrowIfNull(@event);

            await ExecuteAsync(@event, cancellationToken);

            _logger.LogInformation("Finishing handling the event type {Type}: {Event}", typeof(T).Name, @event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    protected abstract Task ExecuteAsync(T @event, CancellationToken cancellationToken = default);
}