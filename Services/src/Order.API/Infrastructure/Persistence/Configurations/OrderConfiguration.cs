using KubeFood.Core.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Order.API.Infrastructure.Persistence.Configurations;

internal class OrderConfiguration : BaseEntityConfiguration<Domain.Order>
{
    public override void Configure(EntityTypeBuilder<Domain.Order> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.CustomerId)
               .IsRequired();

        builder.Property(o => o.Status)
               .IsRequired();

        builder.HasIndex(o => o.Status);

        builder.Property(o => o.PaymentType)
               .IsRequired();

        builder.Property(o => o.TrackingCode)
               .IsRequired(false);

        builder.Property(o => o.FailureReason)
               .IsRequired(false);

        builder.OwnsOne(o => o.DeliveryAddress,
            address =>
            {
                address.Property(a => a.Street)
                       .HasColumnName("Street")
                       .HasMaxLength(100)
                       .IsRequired();

                address.Property(a => a.City)
                       .HasColumnName("City")
                       .HasMaxLength(50)
                       .IsRequired();

                address.Property(a => a.State)
                       .HasColumnName("State")
                       .HasMaxLength(25)
                       .IsRequired();

                address.Property(a => a.ZipCode)
                       .HasColumnName("ZipCode")
                       .HasMaxLength(8)
                       .IsRequired();
            });

        builder.HasMany(o => o.Items)
               .WithOne(i => i.Order)
               .HasForeignKey(i => i.OrderId)
               .IsRequired();
    }
}