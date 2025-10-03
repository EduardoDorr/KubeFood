using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;

namespace KubeFood.Delivery.Worker.Infrastructure.Persistence;

public class WorkerDbContext : DbContext
{
    public DbSet<InboxMessage<ObjectId>> InboxMessages { get; set; }
    public DbSet<OutboxMessage<ObjectId>> OutboxMessages { get; set; }

    public WorkerDbContext(DbContextOptions<WorkerDbContext> options)
        : base(options)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration<ObjectId>());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration<ObjectId>());
    }
}