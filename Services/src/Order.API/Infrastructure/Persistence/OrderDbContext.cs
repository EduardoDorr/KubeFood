using KubeFood.Core.Persistence.Configurations;
using KubeFood.Core.Persistence.Outbox;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Order.API.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public DbSet<Domain.Order> Orders { get; set; }
    public DbSet<Domain.OrderItem> OrderItems { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}