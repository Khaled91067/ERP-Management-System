
namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Purchasing.PurchaseOrders;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PurchaseOrderConfiguration
    : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(
            EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.Property(po => po.TotalAmount)
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

            builder.HasData(
                new
                {
                    Id = 1,
                    SupplierId = 1,
                    OrderDate = new DateTime(2025, 5, 20),
                    ExpectedDelivery = new DateTime(2025, 5, 28),
                    Status = PurchaseOrderStatus.Received,
                    TotalAmount = 5200m,
                    IsDeleted = false, CreatedAt = DateTimeOffset.Parse("2024-01-01T00:00:00Z"), CreatedBy = "System"
                },
                new
                {
                    Id = 2,
                    SupplierId = 2,
                    OrderDate = new DateTime(2025, 6, 1),
                    ExpectedDelivery = new DateTime(2025, 6, 10),
                    Status = PurchaseOrderStatus.Approved,
                    TotalAmount = 1880m,
                    IsDeleted = false, CreatedAt = DateTimeOffset.Parse("2024-01-01T00:00:00Z"), CreatedBy = "System"
                },
                new
                {
                    Id = 3,
                    SupplierId = 3,
                    OrderDate = new DateTime(2025, 6, 5),
                    ExpectedDelivery = new DateTime(2025, 6, 20),
                    Status = PurchaseOrderStatus.Draft,
                    TotalAmount = 1700m,
                    IsDeleted = false, CreatedAt = DateTimeOffset.Parse("2024-01-01T00:00:00Z"), CreatedBy = "System"
                });
        }
    }
}
