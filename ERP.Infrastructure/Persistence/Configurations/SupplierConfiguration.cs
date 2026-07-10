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

            builder.HasData(
                new
                {
                    Id = 1,
                    CompanyName = "Global Tech Distributors",
                    ContactName = "Maya Peterson",
                    Email = "sales@globaltechdist.com",
                    Phone = "+1-646-555-0133",
                    PaymentTerms = "Net 30"
                },
                new
                {
                    Id = 2,
                    CompanyName = "Metro Office Supply",
                    ContactName = "Daniel Cooper",
                    Email = "orders@metroofficesupply.com",
                    Phone = "+44-20-5550-3344",
                    PaymentTerms = "Net 15"
                },
                new
                {
                    Id = 3,
                    CompanyName = "Prime Industrial Parts",
                    ContactName = "Nadia Rahman",
                    Email = "procurement@primeindustrialparts.com",
                    Phone = "+971-4-555-0220",
                    PaymentTerms = "Due on receipt"
                });
        }
    }
}
