using ERP.Application.Features.Authentication.DTOs;
using MediatR;

namespace ERP.Application.Features.Authentication.Commands.Models;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;