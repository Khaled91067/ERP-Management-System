using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class InvoiceLineConfiguration: IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            builder.Property(il => il.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(il => il.UnitPrice)
                   .HasPrecision(18, 2);

            builder.Property(il => il.TaxRate)
                   .HasPrecision(5, 2);

            builder.ToTable(t => t
                   .HasCheckConstraint(
                    "CK_InvoiceLines_TaxRate",
                    "[TaxRate] >= 0 AND [TaxRate] <= 100"));

            builder.HasOne(il => il.Invoice)
                   .WithMany(i => i.InvoiceLines)
                   .HasForeignKey(il => il.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new
                {
                    Id = 1,
                    InvoiceId = 1,
                    Description = "Laser Printer - Model LP850",
                    Quantity = 2,
                    UnitPrice = 850m,
                    TaxRate = 5m
                },
                new
                {
                    Id = 2,
                    InvoiceId = 1,
                    Description = "Wireless Barcode Scanner - Model WBS320",
                    Quantity = 1,
                    UnitPrice = 320m,
                    TaxRate = 5m
                },
                new
                {
                    Id = 3,
                    InvoiceId = 2,
                    Description = "Ergonomic Office Chair - Model OC240",
                    Quantity = 4,
                    UnitPrice = 240m,
                    TaxRate = 0m
                },
                new
                {
                    Id = 4,
                    InvoiceId = 2,
                    Description = "Heavy Duty Carton Box Pack",
                    Quantity = 4,
                    UnitPrice = 35m,
                    TaxRate = 0m
                },
                new
                {
                    Id = 5,
                    InvoiceId = 3,
                    Description = "Industrial Sensor - Model IS180",
                    Quantity = 2,
                    UnitPrice = 180m,
                    TaxRate = 15m
                });
        }
    }
}
