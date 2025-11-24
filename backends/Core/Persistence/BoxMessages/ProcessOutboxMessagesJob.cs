using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class ProcessOutboxMessagesJob<TId, TDbContext>
    : ProcessBoxMessagesJobBase<OutboxMessage<TId>, TId, OutboxMessageOptions, TDbContext>
    where TDbContext : DbContext
{
    public ProcessOutboxMessagesJob(
        IServiceProvider provider,
        ILogger<ProcessOutboxMessagesJob<TId, TDbContext>> logger,
        IOptions<OutboxMessageOptions>? options = null)
        : base(provider, logger, options) { }
}