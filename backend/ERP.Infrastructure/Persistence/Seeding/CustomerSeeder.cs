namespace ERP.Infrastructure.Persistence.Seeding;

using ERP.Domain.Sales.Customers;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class CustomerSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public CustomerSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class CustomerSeedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Customers.AnyAsync(cancellationToken)) return;

        var dtos = await SeedHelper.ReadSeedDataAsync<CustomerSeedDto>("customers.json");
        foreach (var dto in dtos)
        {
            var customer = new Customer(dto.Name, dto.Email, dto.Phone, dto.Address, dto.City, dto.Country, dto.TaxId);
            await _context.Customers.AddAsync(customer, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
