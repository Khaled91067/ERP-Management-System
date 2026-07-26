using ERP.Domain.Entities;

namespace ERP.Application.Abstractions.Repositories;

public interface IPurchaseOrderRepository
    : IGenericRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default);
}