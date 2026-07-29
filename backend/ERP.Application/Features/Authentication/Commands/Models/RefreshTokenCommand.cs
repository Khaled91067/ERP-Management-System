
namespace ERP.Application.Features.Authentication.Commands.Models;

using global::ERP.Application.Features.Authentication.DTOs;

using MediatR;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;
