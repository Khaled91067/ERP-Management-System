
namespace ERP.Infrastructure.Persistence.Configurations
{
    using ERP.Domain.Identity.Users;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasIndex(x => x.Token)
                   .IsUnique();

            builder.HasOne(x => x.User)
           .WithMany(x => x.RefreshTokens)
           .HasForeignKey(x => x.UserId);

        }

    }
}
