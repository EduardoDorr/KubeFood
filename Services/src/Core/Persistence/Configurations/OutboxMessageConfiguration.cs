using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KubeFood.Core.Persistence.Outbox;

namespace KubeFood.Core.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public virtual void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(p => p.Type)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(p => p.Processed)
               .IsRequired();

        builder.HasIndex(p => p.Processed);

        builder.Property(p => p.Error)
               .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
               .IsRequired();

        builder.Property(b => b.ProcessedAt);
    }
}