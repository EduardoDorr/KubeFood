﻿using Catalog.API.Domain;

using Core.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MongoDB.EntityFrameworkCore.Extensions;

namespace Catalog.API.Infrastructure.Persistence.Configurations;

internal class ProductConfiguration : BaseMongoEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.ToCollection("products");

        builder.HasIndex(p => p.Uuid)
               .IsUnique();

        builder.Property(p => p.Uuid)
               .HasElementName("uuid")
               .IsRequired();

        builder.Property(p => p.Name)
               .HasElementName("name")
               .IsRequired();

        builder.Property(p => p.Description)
               .HasElementName("description");

        builder.Property(p => p.Category)
               .HasElementName("category")
               .IsRequired();

        builder.Property(p => p.ImageUrl)
               .HasElementName("imageUrl");

        builder.Property(p => p.Value)
               .HasElementName("value")
               .IsRequired();

        builder.Property(p => p.Weight)
               .HasElementName("weight")
               .IsRequired();
    }
}