using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence.Repositories;

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

    public async Task<IEnumerable<Employee>> GetEmployeesWithDepartmentAsync(
        int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> query = _context.Employees
            .Include(e => e.Department);

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
