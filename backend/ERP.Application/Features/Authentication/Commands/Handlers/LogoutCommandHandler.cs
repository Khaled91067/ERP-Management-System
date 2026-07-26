using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Exceptions;
using ERP.Application.Features.Authentication.Commands.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Application.Features.Authentication.Commands.Handlers;

public sealed class LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository,IUnitOfWork unitOfWork) :
    IRequestHandler<LogoutCommand>
{

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken,cancellationToken);

        if (refreshToken is null)
            throw new UnauthorizedException("Invalid refresh token.");

        if (!refreshToken.IsActive)
            throw new UnauthorizedException("Refresh token is no longer active.");

        refreshToken.RevokedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

