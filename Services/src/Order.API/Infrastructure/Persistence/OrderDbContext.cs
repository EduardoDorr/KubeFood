using KubeFood.Core.Persistence.Configurations;
using KubeFood.Core.Persistence.OutboxInbox;

using Microsoft.EntityFrameworkCore;

namespace KubeFood.Order.API.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public DbSet<Domain.Order> Orders { get; set; }
    public DbSet<Domain.OrderItem> OrderItems { get; set; }
    public DbSet<Domain.OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<InboxMessage<int>> InboxMessages { get; set; }
    public DbSet<OutboxMessage<int>> OutboxMessages { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration<int>());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration<int>());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}