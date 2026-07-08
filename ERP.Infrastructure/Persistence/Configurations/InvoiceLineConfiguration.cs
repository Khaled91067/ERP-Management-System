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
        }
    }
}
