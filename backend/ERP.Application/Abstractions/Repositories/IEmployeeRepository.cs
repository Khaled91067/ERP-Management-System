namespace ERP.Application.Abstractions.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Domain.HR.Employees;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByIdWithDepartmentAsync(
        int id,
        CancellationToken cancellationToken = default);
}
