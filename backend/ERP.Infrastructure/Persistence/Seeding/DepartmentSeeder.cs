namespace ERP.Infrastructure.Persistence.Seeding;

using ERP.Domain.HR.Departments;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class DepartmentSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public DepartmentSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class DepartmentSeedDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Departments.AnyAsync(cancellationToken)) return;

        var dtos = await SeedHelper.ReadSeedDataAsync<DepartmentSeedDto>("departments.json");
        foreach (var dto in dtos)
        {
            var dept = new Department(dto.Name);
            await _context.Departments.AddAsync(dept, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
