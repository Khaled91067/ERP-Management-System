
namespace ERP.Infrastructure.Persistence.Configurations
{
    using System;

    using ERP.Domain.HR.Employees;
    using ERP.Domain.Shared.ValueObjects;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Email)
                   .HasConversion(e => e.Value, v => new Email(v))
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(e => e.Phone)
                .HasMaxLength(30);

            builder.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Salary)
                   .HasConversion(m => m.Amount, v => new Money(v))
                   .HasPrecision(18, 2);

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new
                {
                    Id = 1,
                    FirstName = "Layla",
                    LastName = "Hassan",
                    Email = "layla.hassan@erpco.com",
                    Phone = "+1-212-555-0101",
                    DepartmentId = 1,
                    Position = "Sales Executive",
                    HireDate = new DateTime(2023, 3, 12),
                    Salary = 14500m,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    FirstName = "Omar",
                    LastName = "Farouk",
                    Email = "omar.farouk@erpco.com",
                    Phone = "+1-212-555-0102",
                    DepartmentId = 2,
                    Position = "Operations Supervisor",
                    HireDate = new DateTime(2022, 8, 1),
                    Salary = 16800m,
                    IsDeleted = false
                },
                new
                {
                    Id = 3,
                    FirstName = "Rania",
                    LastName = "Khaled",
                    Email = "rania.khaled@erpco.com",
                    Phone = "+1-212-555-0103",
                    DepartmentId = 3,
                    Position = "Finance Specialist",
                    HireDate = new DateTime(2021, 11, 18),
                    Salary = 15500m,
                    IsDeleted = false
                },
                new
                {
                    Id = 4,
                    FirstName = "Tarek",
                    LastName = "Youssef",
                    Email = "tarek.youssef@erpco.com",
                    Phone = "+1-212-555-0104",
                    DepartmentId = 4,
                    Position = "Procurement Officer",
                    HireDate = new DateTime(2024, 2, 5),
                    Salary = 13200m,
                    IsDeleted = false
                });
        }
    }
}
