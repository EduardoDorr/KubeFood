using KubeFood.Core.Persistence.Configurations;
using KubeFood.Order.API.Domain;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Order.API.Infrastructure.Persistence.Configurations;

public class OrderStatusHistoryConfiguration : BaseEntityConfiguration<OrderStatusHistory, int>
{
    public override void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.OrderId)
               .IsRequired();

        builder.Property(o => o.Status)
               .IsRequired();

        builder.Property(o => o.FailureReason)
               .IsRequired(false);
    }
}