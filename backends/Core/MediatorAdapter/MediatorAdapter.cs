using KubeFood.Core.CommandQueries;
using KubeFood.Core.Events;

namespace KubeFood.Core.MediatorAdapter;

public sealed class MediatorAdapter : IMediator
{
    private readonly IRequestDispatcher _requestDispatcher;
    private readonly IEventDispatcher _eventDispatcher;

    public MediatorAdapter(IRequestDispatcher requestDispatcher, IEventDispatcher eventDispatcher)
    {
        _requestDispatcher = requestDispatcher;
        _eventDispatcher = eventDispatcher;
    }

    /// <summary>
    /// Asynchronously send a request to a single handler
    /// </summary>
    /// <typeparam name="TResult">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    public async Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        => await _requestDispatcher.DispatchAsync(request, cancellationToken);

    /// <summary>
    /// Asynchronously send a notification to multiple handlers
    /// </summary>
    /// <param name="notification">Notification object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation.</returns>
    public async Task Publish<TEvent>(TEvent notification, CancellationToken cancellationToken = default) where TEvent : IEvent
        => await _eventDispatcher.DispatchAsync(notification, cancellationToken);
}