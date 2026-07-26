using ERP.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Abstractions.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<Department?> GetByIdWithEmployeesAsync(
        int id,
        CancellationToken cancellationToken = default);
}
