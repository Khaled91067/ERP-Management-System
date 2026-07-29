namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Roles;

using MediatR;

public sealed class DeleteRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        if (role is null) return false;
        if (await roleRepository.HasUsersAsync(role.Id, cancellationToken))
            throw new InvalidOperationException("A role assigned to users cannot be deleted.");

        roleRepository.Delete(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

