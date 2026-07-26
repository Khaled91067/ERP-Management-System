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

            builder.HasOne(pl => pl.Product)
                .WithMany(p => p.PurchaseLines)
                .HasForeignKey(pl => pl.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new
                {
                    Id = 1,
                    PurchaseOrderId = 1,
                    ProductId = 1,
                    Quantity = 5,
                    UnitCost = 620m
                },
                new
                {
                    Id = 2,
                    PurchaseOrderId = 1,
                    ProductId = 3,
                    Quantity = 10,
                    UnitCost = 210m
                },
                new
                {
                    Id = 3,
                    PurchaseOrderId = 2,
                    ProductId = 2,
                    Quantity = 8,
                    UnitCost = 145m
                },
                new
                {
                    Id = 4,
                    PurchaseOrderId = 2,
                    ProductId = 5,
                    Quantity = 40,
                    UnitCost = 18m
                },
                new
                {
                    Id = 5,
                    PurchaseOrderId = 3,
                    ProductId = 4,
                    Quantity = 10,
                    UnitCost = 110m
                },
                new
                {
                    Id = 6,
                    PurchaseOrderId = 3,
                    ProductId = 6,
                    Quantity = 20,
                    UnitCost = 30m
                });
        }
    }

