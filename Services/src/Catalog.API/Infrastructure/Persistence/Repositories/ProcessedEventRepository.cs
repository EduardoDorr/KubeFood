using KubeFood.Catalog.API.Application.ProcessedEvents;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Catalog.API.Infrastructure.Persistence.Repositories;

public class ProcessedEventRepository : IProcessedEventRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProcessedEventRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> HasBeenProcessedAsync(string eventId, string eventType, CancellationToken cancellationToken = default)
    {
        return _dbContext.ProcessedEvents
            .AnyAsync(pe => pe.EventId == eventId && pe.EventType == eventType, cancellationToken);
    }

    public async Task MarkAsProcessedAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
    }
}