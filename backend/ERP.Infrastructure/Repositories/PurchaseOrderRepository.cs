
namespace ERP.Infrastructure.Persistence.Repositories;

using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

public sealed class PurchaseOrderRepository
    : GenericRepository<PurchaseOrder>,
      IPurchaseOrderRepository
{
    private readonly AppDbContext _context;

    public PurchaseOrderRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.PurchaseOrderLines)
            .FirstOrDefaultAsync(
                po => po.Id == id,
                cancellationToken);
    }
}