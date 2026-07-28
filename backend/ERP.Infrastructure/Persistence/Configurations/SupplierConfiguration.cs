
namespace ERP.Infrastructure.Persistence.Configurations
{
    using System;

    using ERP.Domain.Purchasing.Suppliers;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                   .HasConversion(e => e.Value, v => new Email(v))
                   .HasMaxLength(256);

            builder.Property(s => s.Phone)
                .HasMaxLength(30);

            builder.Property(s => s.PaymentTerms)
                .HasMaxLength(500);


        }
    }
}
