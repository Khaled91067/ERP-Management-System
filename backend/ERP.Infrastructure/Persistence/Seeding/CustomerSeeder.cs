namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Sales.Customers;

using Microsoft.EntityFrameworkCore;

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
        var dtos = await SeedHelper.ReadSeedDataAsync<CustomerSeedDto>("customers.json");
        if (dtos.Count == 0) return;

        var existingEmails = await _context.Customers
            .Select(c => c.Email.Value)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingEmails, StringComparer.OrdinalIgnoreCase);
        var addedAny = false;

        foreach (var dto in dtos)
        {
            var trimmedEmail = dto.Email?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(trimmedEmail) && !existingSet.Contains(trimmedEmail))
            {
                var customer = new Customer(
                    dto.Name,
                    trimmedEmail,
                    dto.Phone ?? string.Empty,
                    dto.Address ?? string.Empty,
                    dto.City ?? string.Empty,
                    dto.Country ?? string.Empty,
                    dto.TaxId ?? string.Empty);

                await _context.Customers.AddAsync(customer, cancellationToken);
                existingSet.Add(trimmedEmail);
                addedAny = true;
            }
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
