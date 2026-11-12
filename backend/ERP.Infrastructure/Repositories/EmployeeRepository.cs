namespace ERP.Infrastructure.Persistence.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Repositories;
using ERP.Domain.HR.Employees;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public sealed class EmployeeRepository
    : GenericRepository<Employee>,
      IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdWithDepartmentAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(
                e => e.Id == id,
                cancellationToken);
    }
}
