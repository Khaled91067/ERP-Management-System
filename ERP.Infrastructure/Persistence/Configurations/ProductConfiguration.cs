using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ERP.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Sku)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(p => p.Sku)
                   .IsUnique();

            builder.Property(p => p.UnitPrice)
                   .HasPrecision(18, 2);

            builder.Property(p => p.CostPrice)
                   .HasPrecision(18, 2);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new
                {
                    Id = 1,
                    Name = "Laser Printer",
                    Sku = "PRN-LP850",
                    CategoryId = 2,
                    UnitPrice = 850m,
                    CostPrice = 620m,
                    StockQuantity = 18,
                    ReorderLevel = 5
                },
                new
                {
                    Id = 2,
                    Name = "Ergonomic Office Chair",
                    Sku = "CHR-OC240",
                    CategoryId = 2,
                    UnitPrice = 240m,
                    CostPrice = 145m,
                    StockQuantity = 30,
                    ReorderLevel = 10
                },
                new
                {
                    Id = 3,
                    Name = "Wireless Barcode Scanner",
                    Sku = "SCN-WBS320",
                    CategoryId = 1,
                    UnitPrice = 320m,
                    CostPrice = 210m,
                    StockQuantity = 14,
                    ReorderLevel = 6
                },
                new
                {
                    Id = 4,
                    Name = "Industrial Sensor",
                    Sku = "SNS-IS180",
                    CategoryId = 3,
                    UnitPrice = 180m,
                    CostPrice = 110m,
                    StockQuantity = 40,
                    ReorderLevel = 12
                },
                new
                {
                    Id = 5,
                    Name = "Heavy Duty Carton Box Pack",
                    Sku = "BOX-HD035",
                    CategoryId = 4,
                    UnitPrice = 35m,
                    CostPrice = 18m,
                    StockQuantity = 150,
                    ReorderLevel = 50
                },
                new
                {
                    Id = 6,
                    Name = "Safety Helmet",
                    Sku = "SFT-HM055",
                    CategoryId = 3,
                    UnitPrice = 55m,
                    CostPrice = 30m,
                    StockQuantity = 80,
                    ReorderLevel = 20
                });
        }
    }
}
