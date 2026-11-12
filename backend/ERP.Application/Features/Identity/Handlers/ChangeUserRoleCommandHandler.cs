namespace ERP.Application.Features.Identity.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;
using ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeUserRoleCommand, bool>
{
    public async Task<bool> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;
        if (await roleRepository.GetByIdAsync(request.RoleId, cancellationToken) is null)
            throw new NotFoundException("Role", request.RoleId);

        user.AssignRole(request.RoleId);

        userRepository.Update(user);
        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
