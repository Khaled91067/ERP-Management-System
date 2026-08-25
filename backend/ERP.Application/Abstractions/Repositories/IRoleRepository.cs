

namespace ERP.Application.Abstractions.Repositories
{
    using ERP.Domain.Identity.Roles;

    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name,CancellationToken cancellationToken = default);

        Task<bool> HasUsersAsync(int roleId, CancellationToken cancellationToken = default);
    }
}

