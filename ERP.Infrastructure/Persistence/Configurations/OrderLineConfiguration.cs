using ERP.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class OrderLineConfiguration: IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.Property(ol => ol.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(ol => ol.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.ToTable(t =>
        t.HasCheckConstraint(
            "CK_OrderLines_DiscountPercentage",
            "[DiscountPercentage] >= 0 AND [DiscountPercentage] <= 100"
        ));

        builder.HasOne(ol => ol.Order)
            .WithMany(o => o.OrderLines)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ol => ol.Product)
            .WithMany(p => p.OrderLines)
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(
            new
            {
                Id = 1,
                OrderId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 850m,
                DiscountPercentage = 0m
            },
            new
            {
                Id = 2,
                OrderId = 1,
                ProductId = 3,
                Quantity = 1,
                UnitPrice = 320m,
                DiscountPercentage = 5m
            },
            new
            {
                Id = 3,
                OrderId = 2,
                ProductId = 2,
                Quantity = 4,
                UnitPrice = 240m,
                DiscountPercentage = 0m
            },
            new
            {
                Id = 4,
                OrderId = 2,
                ProductId = 5,
                Quantity = 4,
                UnitPrice = 35m,
                DiscountPercentage = 0m
            },
            new
            {
                Id = 5,
                OrderId = 3,
                ProductId = 4,
                Quantity = 2,
                UnitPrice = 180m,
                DiscountPercentage = 0m
            },
            new
            {
                Id = 6,
                OrderId = 3,
                ProductId = 6,
                Quantity = 2,
                UnitPrice = 55m,
                DiscountPercentage = 0m
            });
    }
}