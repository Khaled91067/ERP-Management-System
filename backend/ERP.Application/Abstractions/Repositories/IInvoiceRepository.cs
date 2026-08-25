
namespace ERP.Application.Abstractions.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Sales.Invoices;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<Invoice?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Invoice>> GetInvoicesByCustomerAsync(
        int? customerId = null,
        CancellationToken cancellationToken = default);
}

