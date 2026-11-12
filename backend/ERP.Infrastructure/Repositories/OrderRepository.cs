namespace ERP.Infrastructure.Persistence.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Sales.Orders;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public sealed class OrderRepository
    : GenericRepository<Order>,
      IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
            .FirstOrDefaultAsync(
                o => o.Id == id,
                cancellationToken);
    }
}
