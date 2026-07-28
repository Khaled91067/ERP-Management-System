
namespace ERP.Application.Abstractions.Repositories;

using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Sales.Customers;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByIdWithOrdersAndInvoicesAsync(
        int id,
        CancellationToken cancellationToken = default);
}
