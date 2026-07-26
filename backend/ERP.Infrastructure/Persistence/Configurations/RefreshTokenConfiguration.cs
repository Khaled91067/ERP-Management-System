using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Persistence.Configurations
{
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
