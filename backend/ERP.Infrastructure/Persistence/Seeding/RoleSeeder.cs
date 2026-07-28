namespace ERP.Infrastructure.Persistence.Seeding;

using ERP.Domain.Identity.Roles;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class RoleSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public RoleSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Roles.AnyAsync(cancellationToken)) return;

        var roles = await SeedHelper.ReadSeedDataAsync<Role>("roles.json");
        if (roles.Count > 0)
        {
            await _context.Roles.AddRangeAsync(roles, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
