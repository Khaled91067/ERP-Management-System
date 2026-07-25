using ERP.Application.Abstractions.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ERP.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name 
                            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
}
