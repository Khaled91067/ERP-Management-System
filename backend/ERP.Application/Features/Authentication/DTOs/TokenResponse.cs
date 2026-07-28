
namespace ERP.Application.Features.Authentication.DTOs;

public sealed record TokenResponse(string AccessToken,string RefreshToken);