using ERP.Domain.Entities.Orders;
using ERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration: IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalAmount)
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

            builder.HasData(
                new
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = new DateTime(2025, 6, 10),
                    Status = OrderStatus.Shipped,
                    PaymentMethod = PaymentMethod.CreditCard,
                    ShippingAddress = "1200 Market Street, Suite 800, New York, NY",
                    TotalAmount = 2004m
                },
                new
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = new DateTime(2025, 6, 12),
                    Status = OrderStatus.Delivered,
                    PaymentMethod = PaymentMethod.Cash,
                    ShippingAddress = "44 King William Street, London",
                    TotalAmount = 1100m
                },
                new
                {
                    Id = 3,
                    CustomerId = 3,
                    OrderDate = new DateTime(2025, 6, 14),
                    Status = OrderStatus.Processing,
                    PaymentMethod = PaymentMethod.MobilePayment,
                    ShippingAddress = "Business Bay Tower 18, Dubai",
                    TotalAmount = 470m
                });
        }
    }



}
