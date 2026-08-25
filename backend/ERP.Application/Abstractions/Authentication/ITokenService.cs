
namespace ERP.Application.Abstractions.Authentication;

using ERP.Domain.Identity.Users;

public interface ITokenService
{
    string GenerateToken(User user);

    string GenerateRefreshToken();
}
