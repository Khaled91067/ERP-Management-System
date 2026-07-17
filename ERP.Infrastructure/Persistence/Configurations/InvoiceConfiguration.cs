using ERP.Domain.Entities;
using ERP.Domain.Enums;
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

            builder.HasData(
                new
                {
                    Id = 1,
                    OrderId = 1,
                    CustomerId = 1,
                    InvoiceDate = new DateTime(2025, 6, 11),
                    DueDate = new DateTime(2025, 6, 26),
                    Status = InvoiceStatus.Sent,
                    TotalAmount = 2004m,
                    PaidAt = (DateTime?)null
                },
                new
                {
                    Id = 2,
                    OrderId = 2,
                    CustomerId = 2,
                    InvoiceDate = new DateTime(2025, 6, 13),
                    DueDate = new DateTime(2025, 6, 28),
                    Status = InvoiceStatus.Paid,
                    TotalAmount = 1100m,
                    PaidAt = new DateTime(2025, 6, 15)
                },
                new
                {
                    Id = 3,
                    OrderId = 3,
                    CustomerId = 3,
                    InvoiceDate = new DateTime(2025, 6, 15),
                    DueDate = new DateTime(2025, 6, 30),
                    Status = InvoiceStatus.Draft,
                    TotalAmount = 470m,
                    PaidAt = (DateTime?)null
                });
        }
    }

}