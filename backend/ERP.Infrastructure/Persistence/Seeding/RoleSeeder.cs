namespace ERP.Infrastructure.Persistence.Seeding;

using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Identity.Permissions;
using ERP.Domain.Identity.Roles;

using Microsoft.EntityFrameworkCore;

public class RoleSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public RoleSeeder(AppDbContext context)
    {
        _context = context;
    }

    private sealed class RoleSeedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Permissions { get; set; } = string.Empty;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!await _context.Roles.AnyAsync(cancellationToken))
        {
            var dtos = await SeedHelper.ReadSeedDataAsync<RoleSeedDto>("roles.json");
            if (dtos.Count > 0)
            {
                foreach (var dto in dtos)
                {
                    await _context.Roles.AddAsync(new Role(dto.Name, dto.Permissions), cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            return;
        }

        var existingRoles = await _context.Roles.ToListAsync(cancellationToken);
        var modified = false;
        foreach (var role in existingRoles)
        {
            if (string.IsNullOrWhiteSpace(role.Permissions) || role.Permissions == "[]")
            {
                var defaultPerms = Permissions.GetDefaultPermissions(role.Name);
                if (defaultPerms.Count > 0)
                {
                    role.SetPermissions(defaultPerms);
                    modified = true;
                }
            }
        }

        if (modified)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
