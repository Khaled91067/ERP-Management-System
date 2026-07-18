using ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Application.Abstractions.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token,CancellationToken cancellationToken);

        Task AddAsync(RefreshToken refreshToken,CancellationToken cancellationToken);
    }
}
