using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

using MongoDB.EntityFrameworkCore.Extensions;

namespace Core.Persistence.Configurations;

public abstract class BaseMongoEntityConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseMongoEntity
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(p => p.Id);

        builder.Property(b => b.IsActive)
               .IsRequired();

        builder.HasIndex(b => b.IsActive);

        builder.HasQueryFilter(b => b.IsActive);

        builder.Property(b => b.CreatedAt)
               .IsRequired();

        builder.Property(b => b.UpdatedAt)
               .IsRequired();
    }
}