using ERP.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Abstractions.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByIdWithOrdersAndInvoicesAsync(
        int id,
        CancellationToken cancellationToken = default);
}
