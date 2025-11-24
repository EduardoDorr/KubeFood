using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<InboxMessage<ObjectId>> InboxMessages { get; set; }
    public DbSet<OutboxMessage<ObjectId>> OutboxMessages { get; set; }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration<ObjectId>());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration<ObjectId>());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}