using ERP.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Abstractions.Repositories;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByIdWithDepartmentAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Employee>> GetEmployeesWithDepartmentAsync(
        int? departmentId = null,
        CancellationToken cancellationToken = default);
}
