using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

    public class PurchaseLineConfiguration : IEntityTypeConfiguration<PurchaseLine>
    {
        public void Configure(EntityTypeBuilder<PurchaseLine> builder)
        {
            builder.Property(pl => pl.UnitCost)
                .HasPrecision(18, 2);

            builder.HasOne(pl => pl.PurchaseOrder)
                .WithMany(po => po.PurchaseLines)
                .HasForeignKey(pl => pl.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pl => pl.Product)
                .WithMany(p => p.PurchaseLines)
                .HasForeignKey(pl => pl.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

