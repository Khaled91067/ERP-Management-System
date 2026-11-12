namespace ERP.Application.Abstractions.Repositories;

using System.Threading;
using System.Threading.Tasks;
using ERP.Domain.Identity.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsExceptAsync(string email, int userId, CancellationToken cancellationToken = default);

    Task<User?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default);
}
