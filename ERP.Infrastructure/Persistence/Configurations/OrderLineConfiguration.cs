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

    }
}