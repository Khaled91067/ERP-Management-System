
namespace ERP.Application.Abstractions.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Sales.Orders;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> GetOrdersWithCustomerAsync(
        int? customerId = null,
        CancellationToken cancellationToken = default);
}
