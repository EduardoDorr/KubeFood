namespace Core.Interfaces;

public interface IRequestHandler<in TRequest, TResult>
    where TRequest : notnull
    where TResult : notnull
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}