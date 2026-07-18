using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Authentication;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Exceptions;
using ERP.Application.Features.Authentication.Commands.Models;
using ERP.Application.Features.Authentication.DTOs;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Authentication.Commands.Handlers;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null) throw new UnauthorizedException();

        if (refreshToken.ExpiresAt <= DateTime.UtcNow) throw new UnauthorizedException();

        if (refreshToken.RevokedAt is not null) throw new UnauthorizedException();

        var user = refreshToken.User;

        var accessToken = tokenService.GenerateToken(user);

        var newRefreshToken = tokenService.GenerateRefreshToken();

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.ReplacedByToken = newRefreshToken;

        await refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            },
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse(accessToken,newRefreshToken);
    }
}