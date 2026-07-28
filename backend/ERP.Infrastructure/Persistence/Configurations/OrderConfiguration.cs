
namespace ERP.Infrastructure.Persistence.Configurations
{
    using System;

    using ERP.Domain.Sales.Orders;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OrderConfiguration: IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalAmount)
                .HasConversion(m => m.Amount, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Property(o => o.ShippingAddress)
                .IsRequired()
                .HasMaxLength(500); 
                
            builder.Property(o => o.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }



}
