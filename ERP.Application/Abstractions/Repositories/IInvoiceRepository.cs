using ERP.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Abstractions.Repositories;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<Invoice?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Invoice>> GetInvoicesByCustomerAsync(
        int? customerId = null,
        CancellationToken cancellationToken = default);
}
