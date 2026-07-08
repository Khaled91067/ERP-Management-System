using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ERP.Infrastructure.Persistence.Configurations
{

    public class InvoiceConfiguration: IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(i => i.TotalAmount)
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