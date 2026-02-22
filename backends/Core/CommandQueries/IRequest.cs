namespace KubeFood.Core.CommandQueries;

public interface IRequest<TResult> { }

public interface IRequestHandler<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}