
namespace ERP.Application.Features.Authentication.Commands;

using ERP.Application.Features.Authentication.DTOs;

using MediatR;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;
