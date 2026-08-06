namespace ERP.Infrastructure.Persistence.Seeding;

using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Purchasing.Suppliers;

using Microsoft.EntityFrameworkCore;

public class SupplierSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public SupplierSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class SupplierSeedDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = string.Empty;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Suppliers.AnyAsync(cancellationToken)) return;

        var dtos = await SeedHelper.ReadSeedDataAsync<SupplierSeedDto>("suppliers.json");
        foreach (var dto in dtos)
        {
            var sup = new Supplier(dto.CompanyName, dto.ContactName, dto.Email, dto.Phone, dto.PaymentTerms);
            await _context.Suppliers.AddAsync(sup, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
