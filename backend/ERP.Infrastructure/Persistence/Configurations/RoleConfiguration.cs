using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.Property(r => r.Permissions)
                .IsRequired();

            builder.HasData(
                new
                {
                    Id = 1,
                    Name = "Administrator",
                    Permissions = "[\"Users.Read\",\"Users.Write\",\"Orders.Read\",\"Orders.Write\",\"Reports.Read\",\"Reports.Write\"]"
                },
                new
                {
                    Id = 2,
                    Name = "Sales Manager",
                    Permissions = "[\"Customers.Read\",\"Customers.Write\",\"Orders.Read\",\"Orders.Write\",\"Invoices.Read\"]"
                },
                new
                {
                    Id = 3,
                    Name = "Warehouse Clerk",
                    Permissions = "[\"Products.Read\",\"PurchaseOrders.Read\",\"PurchaseOrders.Write\",\"Inventory.Update\"]"
                });
        }
    }
}
