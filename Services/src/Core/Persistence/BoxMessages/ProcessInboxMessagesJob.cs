using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KubeFood.Core.Persistence.BoxMessages;

public sealed class ProcessInboxMessagesJob<TId, TDbContext>
    : ProcessBoxMessagesJobBase<InboxMessage<TId>, TId, InboxMessageOptions, TDbContext>
    where TDbContext : DbContext
{
    public ProcessInboxMessagesJob(
        IServiceProvider provider,
        ILogger<ProcessInboxMessagesJob<TId, TDbContext>> logger,
        IOptions<InboxMessageOptions>? options = null)
        : base(provider, logger, options) { }
}