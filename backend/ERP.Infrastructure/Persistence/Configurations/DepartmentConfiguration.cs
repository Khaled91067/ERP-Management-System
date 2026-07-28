

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


        }
    }
}
