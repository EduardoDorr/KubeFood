using KubeFood.Core.Persistence.Configurations;
using KubeFood.Inventory.API.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Inventory.API.Infrastructure.Persistence.Configurations;

internal class InventoryItemConfiguration : BaseEntityConfiguration<InventoryItem, int>
{
    public override void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.ProductId)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(o => o.ProductId)
               .IsUnique();

        builder.Property(o => o.Name)
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(o => o.QuantityAvailable)
               .IsRequired();

        builder.Property(o => o.QuantityReserved)
               .IsRequired();

        builder.Property<byte[]>("RowVersion")
               .IsRowVersion()
               .IsConcurrencyToken();

        builder.HasMany(o => o.Movements)
               .WithOne(i => i.InventoryItem)
               .HasForeignKey(i => i.InventoryId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}