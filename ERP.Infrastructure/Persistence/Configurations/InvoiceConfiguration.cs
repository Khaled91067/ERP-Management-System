using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ERP.Infrastructure.Persistence.Configurations
{

    public class InvoiceConfiguration: IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(i => i.TotalAmount)
                   .HasPrecision(18, 2);

            builder.Property(i => i.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);
        }
    }

}