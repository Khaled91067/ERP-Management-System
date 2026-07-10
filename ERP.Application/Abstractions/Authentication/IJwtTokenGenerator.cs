using ERP.Domain.Entities;

namespace ERP.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}