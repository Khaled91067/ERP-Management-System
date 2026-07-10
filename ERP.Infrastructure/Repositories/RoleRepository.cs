using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public sealed class RoleRepository: GenericRepository<Role>, IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name,CancellationToken cancellationToken = default)
        {
            return await _context.Set<Role>()
                .FirstOrDefaultAsync(
                    role => role.Name == name,
                    cancellationToken);
        }
    }
}
