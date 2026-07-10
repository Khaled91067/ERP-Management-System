using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
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

            builder.HasData(
                new
                {
                    Id = 1,
                    Name = "Al Noor Trading Co.",
                    Email = "purchasing@alnoortrading.com",
                    Phone = "+1-212-555-0148",
                    Address = "1200 Market Street, Suite 800",
                    City = "New York",
                    Country = "USA",
                    TaxId = "US-TAX-100245"
                },
                new
                {
                    Id = 2,
                    Name = "BlueWave Retail Ltd.",
                    Email = "accounts@bluewaveretail.com",
                    Phone = "+44-20-5550-2211",
                    Address = "44 King William Street",
                    City = "London",
                    Country = "UK",
                    TaxId = "GB-TAX-778845"
                },
                new
                {
                    Id = 3,
                    Name = "Horizon Construction LLC",
                    Email = "billing@horizonconstruction.com",
                    Phone = "+971-4-555-0198",
                    Address = "Business Bay Tower 18",
                    City = "Dubai",
                    Country = "UAE",
                    TaxId = "AE-TAX-442110"
                });
        }
    }
}
