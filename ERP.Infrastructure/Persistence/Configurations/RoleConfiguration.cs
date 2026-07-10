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
            new Role
            {
                Id = 1,
                Name = "Admin",
                Permissions = "[]"
            },
            new Role
            {
                Id = 2,
                Name = "User",
                Permissions = "[]"
            }
        );
        }
    }
}
