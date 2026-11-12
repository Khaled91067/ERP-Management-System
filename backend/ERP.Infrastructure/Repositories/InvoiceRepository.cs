namespace ERP.Infrastructure.Persistence.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Sales.Invoices;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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
}
