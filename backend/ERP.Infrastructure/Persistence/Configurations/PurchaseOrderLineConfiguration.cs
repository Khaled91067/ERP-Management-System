namespace ERP.Infrastructure.Persistence.Configurations;

using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Domain.Shared.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.Property(pl => pl.UnitCost)
               .HasConversion(m => m.Amount, v => new Money(v))
               .HasPrecision(18, 2);

        builder.HasOne(pl => pl.Product)
            .WithMany(p => p.PurchaseOrderLines)
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.NoAction);


    }
}
