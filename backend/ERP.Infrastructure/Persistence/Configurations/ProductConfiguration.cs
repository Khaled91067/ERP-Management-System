

namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Catalog.Products;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");

            builder.Property(p => p.UnitPrice)
                   .HasConversion(m => m.Amount, v => new Money(v))
                   .HasPrecision(18, 2);

            builder.Property(p => p.CostPrice)
                   .HasConversion(m => m.Amount, v => new Money(v))
                   .HasPrecision(18, 2);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
