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
        }
    }

