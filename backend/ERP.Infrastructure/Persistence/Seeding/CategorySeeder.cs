namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Catalog.Categories;

using Microsoft.EntityFrameworkCore;

public class CategorySeeder : ISeeder
{
    private readonly AppDbContext _context;

    public CategorySeeder(AppDbContext context)
    {
        _context = context;
    }

    private sealed class CategorySeedDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await SeedHelper.ReadSeedDataAsync<CategorySeedDto>("categories.json");
        if (dtos.Count == 0) return;

        var existingNames = await _context.Categories
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
        var addedAny = false;

        foreach (var dto in dtos)
        {
            var trimmedName = dto.Name?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(trimmedName) && !existingSet.Contains(trimmedName))
            {
                await _context.Categories.AddAsync(new Category(trimmedName), cancellationToken);
                existingSet.Add(trimmedName);
                addedAny = true;
            }
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
