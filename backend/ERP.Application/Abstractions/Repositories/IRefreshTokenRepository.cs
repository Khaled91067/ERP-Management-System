
namespace ERP.Application.Abstractions.Repositories
{
    using ERP.Domain.Identity.Users;

    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token,CancellationToken cancellationToken);

        Task AddAsync(RefreshToken refreshToken,CancellationToken cancellationToken);
    }
}
