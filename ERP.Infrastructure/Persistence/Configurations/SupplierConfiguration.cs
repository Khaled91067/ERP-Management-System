using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.Property(s => s.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.ContactName)
                .HasMaxLength(200);

            builder.Property(s => s.Email)
                .HasMaxLength(256);

            builder.Property(s => s.Phone)
                .HasMaxLength(30);

            builder.Property(s => s.PaymentTerms)
                .HasMaxLength(500);
        }
    }
}
