
namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Purchasing.PurchaseOrders;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PurchaseOrderConfiguration
    : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(
            EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.Property(po => po.ExpectedDelivery)
                   .IsRequired();

            builder.Property(po => po.TotalAmount)
                   .HasConversion(m => m.Amount, v => new Money(v))
                   .HasPrecision(18, 2);

            builder.Property(po => po.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(po => po.PurchaseOrderLines)
                .WithOne(pl => pl.PurchaseOrder)
                .HasForeignKey(pl => pl.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(po => po.PurchaseOrderLines)
                .UsePropertyAccessMode(
                    PropertyAccessMode.Field);


        }
    }
}
