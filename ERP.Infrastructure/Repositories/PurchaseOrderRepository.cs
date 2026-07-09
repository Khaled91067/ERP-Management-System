using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence.Repositories;

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
            .Include(po => po.PurchaseLines)
            .FirstOrDefaultAsync(
                po => po.Id == id,
                cancellationToken);
    }
}