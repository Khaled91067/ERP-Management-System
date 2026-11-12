namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
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
        var dtos = await SeedHelper.ReadSeedDataAsync<SupplierSeedDto>("suppliers.json");
        if (dtos.Count == 0) return;

        var existingEmails = await _context.Suppliers
            .Select(s => s.Email.Value)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingEmails, StringComparer.OrdinalIgnoreCase);
        var addedAny = false;

        foreach (var dto in dtos)
        {
            var trimmedEmail = dto.Email?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(trimmedEmail) && !existingSet.Contains(trimmedEmail))
            {
                var sup = new Supplier(
                    dto.CompanyName,
                    trimmedEmail,
                    dto.ContactName ?? string.Empty,
                    dto.Phone ?? string.Empty,
                    dto.PaymentTerms ?? string.Empty);

                await _context.Suppliers.AddAsync(sup, cancellationToken);
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
