

namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.HR.Departments;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => d.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasData(
                new { Id = 1, Name = "Sales", IsDeleted = false },
                new { Id = 2, Name = "Operations", IsDeleted = false },
                new { Id = 3, Name = "Finance", IsDeleted = false },
                new { Id = 4, Name = "Procurement", IsDeleted = false });
        }
    }
}
