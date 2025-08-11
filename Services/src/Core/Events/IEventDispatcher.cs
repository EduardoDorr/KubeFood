namespace KubeFood.Core.Events;

public interface IEventDispatcher
{
    Task DispatchAsync(IEvent @event, CancellationToken cancellationToken = default);
}