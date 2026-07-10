using ERP.Domain.Entities;


namespace ERP.Application.Abstractions.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name,CancellationToken cancellationToken = default);
    }
}
