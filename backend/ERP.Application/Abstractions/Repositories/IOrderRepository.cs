using ERP.Domain.Entities.Orders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Abstractions.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> GetOrdersWithCustomerAsync(
        int? customerId = null,
        CancellationToken cancellationToken = default);
}
