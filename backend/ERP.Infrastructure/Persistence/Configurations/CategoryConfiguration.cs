using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasData(
                new { Id = 1, Name = "Electronics", IsDeleted = false },
                new { Id = 2, Name = "Office Supplies", IsDeleted = false },
                new { Id = 3, Name = "Industrial Equipment", IsDeleted = false },
                new { Id = 4, Name = "Packaging", IsDeleted = false });
        }
    }
}
