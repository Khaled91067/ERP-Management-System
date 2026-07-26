using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository
    : GenericRepository<Customer>,
      ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdWithOrdersAndInvoicesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
    }
}
