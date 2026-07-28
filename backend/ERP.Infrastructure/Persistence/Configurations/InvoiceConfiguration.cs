

namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Sales.Invoices;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class InvoiceConfiguration: IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(i => i.TotalAmount)
                   .HasConversion(m => m.Amount, v => new Money(v))
                   .HasPrecision(18, 2);

            builder.Property(i => i.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

            builder.HasOne(i => i.Customer)
                   .WithMany(c => c.Invoices)
                   .HasForeignKey(i => i.CustomerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(i => i.Order)
                   .WithMany(o => o.Invoices)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.NoAction);


        }
    }

}