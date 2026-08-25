namespace ERP.Infrastructure.Persistence.Seeding;

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

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Categories.AnyAsync(cancellationToken)) return;

        var categories = await SeedHelper.ReadSeedDataAsync<Category>("categories.json");
        if (categories.Count > 0)
        {
            await _context.Categories.AddRangeAsync(categories, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
