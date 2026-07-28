
namespace ERP.Application.Abstractions.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.HR.Employees;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByIdWithDepartmentAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Employee>> GetEmployeesWithDepartmentAsync(
        int? departmentId = null,
        CancellationToken cancellationToken = default);
}
