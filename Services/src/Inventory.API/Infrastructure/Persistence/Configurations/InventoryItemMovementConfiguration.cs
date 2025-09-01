using KubeFood.Core.Persistence.Configurations;
using KubeFood.Inventory.API.Domain;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Inventory.API.Infrastructure.Persistence.Configurations;

internal class InventoryItemMovementConfiguration : BaseEntityConfiguration<InventoryItemMovement, int>
{
    public override void Configure(EntityTypeBuilder<InventoryItemMovement> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.InventoryId)
               .IsRequired();

        builder.Property(b => b.Quantity)
               .IsRequired();

        builder.Property(b => b.Type)
               .IsRequired();

        builder.Property(o => o.OrderId)
               .IsRequired(false);
    }
}