
namespace ERP.Application.Abstractions.Repositories;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Domain.HR.Departments;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<Department?> GetByIdWithEmployeesAsync(
        int id,
        CancellationToken cancellationToken = default);
}

