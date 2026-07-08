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
        }
    }
}
