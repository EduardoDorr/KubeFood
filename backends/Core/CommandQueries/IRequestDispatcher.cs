namespace KubeFood.Core.CommandQueries;

public interface IRequestDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
}