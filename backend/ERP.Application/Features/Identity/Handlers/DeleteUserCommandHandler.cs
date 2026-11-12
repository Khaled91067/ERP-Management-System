namespace ERP.Application.Features.Identity.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Common;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;
using ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;

        if (currentUserService.UserId == user.Id.ToString())
            throw new BusinessRuleValidationException("You cannot delete your own account.");

        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);
        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
