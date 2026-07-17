using ERP.Domain.Entities;

namespace ERP.Application.Abstractions.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsExceptAsync(string email, int userId, CancellationToken cancellationToken = default);

    Task<User?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default);
}
