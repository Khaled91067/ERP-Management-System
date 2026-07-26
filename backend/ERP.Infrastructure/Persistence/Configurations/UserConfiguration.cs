using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new
                {
                    Id = 1,
                    FirstName = "Omar",
                    LastName = "Saleh",
                    Email = "omar.saleh@erpco.com",
                    PasswordHash = "$2a$11$k5X4wQb3G4z7kQv8Ff9M2O1hQ2rM5g7uN6p7v8w9x0y1z2a3b4c5d",
                    RoleId = 1
                },
                new
                {
                    Id = 2,
                    FirstName = "Sara",
                    LastName = "Ibrahim",
                    Email = "sara.ibrahim@erpco.com",
                    PasswordHash = "$2a$11$g2H7nQp4R8s1tV6wX9y0zAaBbCcDdEeFfGgHhIiJjKkLlMmNnOoP",
                    RoleId = 2
                },
                new
                {
                    Id = 3,
                    FirstName = "Ahmed",
                    LastName = "Nasser",
                    Email = "ahmed.nasser@erpco.com",
                    PasswordHash = "$2a$11$7uY2dK9mS1pQ4rT6vW8xZ0AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoQ",
                    RoleId = 3
                });
        }
    }


}
