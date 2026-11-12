namespace ERP.Infrastructure.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Purchasing.Suppliers;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<bool> HasPurchaseOrdersAsync(int supplierId, CancellationToken cancellationToken = default) =>
        _context.PurchaseOrders.AnyAsync(po => po.SupplierId == supplierId, cancellationToken);
}
