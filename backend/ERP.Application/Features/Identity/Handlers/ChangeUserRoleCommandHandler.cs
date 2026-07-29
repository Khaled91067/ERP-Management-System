namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Users;

using MediatR;

public sealed class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeUserRoleCommand, bool>
{
    public async Task<bool> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null) return false;
        if (await roleRepository.GetByIdAsync(request.RoleId) is null)
            throw new InvalidOperationException("Role does not exist.");

        user.AssignRole(request.RoleId);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

