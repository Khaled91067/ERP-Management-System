namespace ERP.Application.Features.Authentication.Commands.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Exceptions;
using global::ERP.Application.Features.Authentication.Commands;
using global::ERP.Application.Features.Authentication.DTOs;
using global::ERP.Domain.Identity.Users;

using MediatR;

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

        refreshToken.Revoke(newRefreshToken);

        await refreshTokenRepository.AddAsync(
            new RefreshToken(newRefreshToken, user.Id, DateTime.UtcNow.AddDays(7)),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse(accessToken, newRefreshToken);
    }
}
