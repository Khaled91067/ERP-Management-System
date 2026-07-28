
namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Sales.Customers;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                   .HasConversion(e => e.Value, v => new Email(v))
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(c => c.Phone)
                .HasMaxLength(30);

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.Property(c => c.Country)
                .HasMaxLength(100);

            builder.Property(c => c.TaxId)
                .HasMaxLength(100);


        }
    }
}
