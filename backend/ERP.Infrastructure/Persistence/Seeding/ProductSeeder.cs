namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Catalog.Products;

using Microsoft.EntityFrameworkCore;

public class ProductSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public ProductSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class ProductSeedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await SeedHelper.ReadSeedDataAsync<ProductSeedDto>("products.json");
        if (dtos.Count == 0) return;

        var existingSkus = await _context.Products
            .Select(p => p.Sku)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingSkus, StringComparer.OrdinalIgnoreCase);

        var categories = await _context.Categories
            .ToDictionaryAsync(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var defaultCategoryId = categories.Values.FirstOrDefault();
        var addedAny = false;

        foreach (var dto in dtos)
        {
            var trimmedSku = dto.Sku?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedSku) || existingSet.Contains(trimmedSku))
            {
                continue;
            }

            var categoryId = 0;
            if (!string.IsNullOrWhiteSpace(dto.CategoryName) && categories.TryGetValue(dto.CategoryName.Trim(), out var resolvedCatId))
            {
                categoryId = resolvedCatId;
            }
            else if (dto.CategoryId > 0 && categories.Values.Contains(dto.CategoryId))
            {
                categoryId = dto.CategoryId;
            }
            else
            {
                categoryId = defaultCategoryId;
            }

            var product = new Product(
                dto.Name,
                trimmedSku,
                categoryId,
                dto.UnitPrice,
                dto.CostPrice,
                dto.ReorderLevel);

            if (dto.StockQuantity > 0)
            {
                product.IncreaseStock(dto.StockQuantity);
            }

            await _context.Products.AddAsync(product, cancellationToken);
            existingSet.Add(trimmedSku);
            addedAny = true;
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
