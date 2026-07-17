using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities.Orders;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence.Repositories;

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

    public async Task<IEnumerable<Order>> GetOrdersWithCustomerAsync(
        int? customerId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderLines);

        if (customerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == customerId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
