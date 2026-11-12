namespace ERP.Infrastructure.Repositories
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ERP.Application.Abstractions.Repositories;
    using ERP.Domain.Identity.Users;
    using ERP.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }

        public async Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }
        }
    }
}
