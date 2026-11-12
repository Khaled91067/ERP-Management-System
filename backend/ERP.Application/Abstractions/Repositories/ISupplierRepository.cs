namespace ERP.Application.Abstractions.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Domain.Purchasing.Suppliers;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<bool> HasPurchaseOrdersAsync(int supplierId, CancellationToken cancellationToken = default);
}
