using ERP.Domain.Entities;
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
    }
}