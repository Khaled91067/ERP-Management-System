
namespace ERP.Application.Abstractions.Repositories;

using global::ERP.Domain.Purchasing.PurchaseOrders;

public interface IPurchaseOrderRepository
    : IGenericRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default);
}
