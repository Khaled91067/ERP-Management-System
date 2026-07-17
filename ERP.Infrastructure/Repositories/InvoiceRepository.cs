using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository
    : GenericRepository<Invoice>,
      IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdWithLinesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines)
            .FirstOrDefaultAsync(
                i => i.Id == id,
                cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerAsync(
        int? customerId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.InvoiceLines);

        if (customerId.HasValue)
        {
            query = query.Where(i => i.CustomerId == customerId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
