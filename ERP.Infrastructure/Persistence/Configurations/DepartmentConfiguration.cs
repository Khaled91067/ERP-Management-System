using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ERP.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => d.Name)
                .IsUnique();

            builder.HasData(
                new { Id = 1, Name = "Sales" },
                new { Id = 2, Name = "Operations" },
                new { Id = 3, Name = "Finance" },
                new { Id = 4, Name = "Procurement" });
        }
    }
}
