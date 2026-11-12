namespace ERP.Application.Abstractions.Repositories
{
    using System.Threading;
    using System.Threading.Tasks;
    using ERP.Domain.Identity.Users;

    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);

        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

        Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken);
    }
}
