using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories;

public sealed class UserRepository: GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Email == email,cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email,CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .AnyAsync( user => user.Email == email,cancellationToken);
    }

    public async Task<bool> EmailExistsExceptAsync(string email, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .AnyAsync(user => user.Email == email && user.Id != userId, cancellationToken);
    }

    public async Task<User?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .Include(user => user.Role)
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ToListAsync(cancellationToken);
    }
}
