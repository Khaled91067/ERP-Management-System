
namespace ERP.Infrastructure.Persistence.Repositories;

using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Sales.Customers;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

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
