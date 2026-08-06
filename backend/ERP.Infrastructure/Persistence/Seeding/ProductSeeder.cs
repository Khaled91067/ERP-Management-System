namespace ERP.Infrastructure.Persistence.Seeding;

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
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Products.AnyAsync(cancellationToken)) return;

        var dtos = await SeedHelper.ReadSeedDataAsync<ProductSeedDto>("products.json");
        foreach (var dto in dtos)
        {
            var product = new Product(dto.Name, dto.Sku, dto.CategoryId, dto.UnitPrice, dto.CostPrice, dto.ReorderLevel);
            if (dto.StockQuantity > 0) product.IncreaseStock(dto.StockQuantity);
            await _context.Products.AddAsync(product, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
