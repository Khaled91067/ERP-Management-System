
namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Sales.Invoices;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class InvoiceLineConfiguration: IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            builder.Property(il => il.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(il => il.UnitPrice)
                   .HasConversion(m => m.Amount, v => new Money(v))
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


        }
    }
}
