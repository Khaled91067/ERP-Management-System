
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


        }
    }
}
