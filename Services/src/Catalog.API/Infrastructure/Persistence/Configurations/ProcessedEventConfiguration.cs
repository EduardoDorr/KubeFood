using KubeFood.Catalog.API.Application.ProcessedEvents;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KubeFood.Catalog.API.Infrastructure.Persistence.Configurations;

public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.EventId)
               .IsRequired();

        builder.Property(b => b.EventType)
               .IsRequired();

        builder.Property(b => b.ProcessedAt)
               .IsRequired();
    }
}