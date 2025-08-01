using KubeFood.Core.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Order.API.Infrastructure.Persistence.Configurations;

internal class OrderItemConfiguration : BaseEntityConfiguration<Domain.OrderItem>
{
    public override void Configure(EntityTypeBuilder<Domain.OrderItem> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.OrderId)
               .IsRequired();

        builder.Property(p => p.ProductId)
               .IsRequired();

        builder.Property(p => p.Name)
               .IsRequired();

        builder.Property(p => p.Price)
               .HasColumnType("numeric(18,2)")
               .IsRequired();

        builder.Property(p => p.Quantity)
               .IsRequired();
    }
}