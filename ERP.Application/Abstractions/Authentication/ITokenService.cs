using ERP.Domain.Entities;

namespace ERP.Application.Abstractions.Authentication;

public interface ITokenService
{
    string GenerateToken(User user);

    string GenerateRefreshToken();
}