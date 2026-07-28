
namespace ERP.Infrastructure.Repositories;

using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Purchasing.Suppliers;
using ERP.Infrastructure.Persistence;

public sealed class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context)
    {
    }
}
