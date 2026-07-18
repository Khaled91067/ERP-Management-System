using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository
    : GenericRepository<Department>,
      IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdWithEmployeesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(
                d => d.Id == id,
                cancellationToken);
    }
}
