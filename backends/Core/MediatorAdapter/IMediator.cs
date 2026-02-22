using KubeFood.Core.CommandQueries;
using KubeFood.Core.Events;

namespace KubeFood.Core.MediatorAdapter;

public interface IMediator
{
    Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
    Task Publish<TEvent>(TEvent notification, CancellationToken cancellationToken = default) where TEvent : IEvent;
}