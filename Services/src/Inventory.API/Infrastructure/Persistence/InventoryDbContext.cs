using KubeFood.Core.Persistence.BoxMessages;
using KubeFood.Core.Persistence.Configurations;
using KubeFood.Inventory.API.Domain;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Inventory.API.Infrastructure.Persistence;

public class InventoryDbContext : DbContext
{
    public DbSet<InventoryItem> Inventory { get; set; }
    public DbSet<InventoryItemMovement> InventoryMovements { get; set; }
    public DbSet<InboxMessage<int>> InboxMessages { get; set; }
    public DbSet<OutboxMessage<int>> OutboxMessages { get; set; }

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration<int>());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration<int>());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
}