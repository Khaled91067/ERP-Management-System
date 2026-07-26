using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;

namespace ERP.Infrastructure.Repositories;

public sealed class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context)
    {
    }
}
