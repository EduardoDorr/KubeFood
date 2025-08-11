using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Core.Persistence.Configurations;

public abstract class BaseBoxMessageConfiguration<TBase, TId> : IEntityTypeConfiguration<TBase> where TBase : BaseBoxMessage<TId>
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(p => p.Type)
               .HasMaxLength(100)
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