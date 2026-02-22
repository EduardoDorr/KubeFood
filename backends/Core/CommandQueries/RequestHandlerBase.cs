using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace KubeFood.Core.CommandQueries;

public abstract class RequestHandlerBase<TRequest, TResult>
    : IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    protected readonly ILogger _logger;

    protected RequestHandlerBase(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting handling {RequestType}", typeof(TRequest).Name);
            var sw = Stopwatch.StartNew();

            if (request is null)
                ArgumentNullException.ThrowIfNull(request);

            var result = await ExecuteAsync(request, cancellationToken);

            sw.Stop();

            _logger.LogInformation("Finishing handling type {RequestType} in {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {RequestType}", typeof(TRequest).Name);
            throw;
        }
    }

    protected abstract Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}