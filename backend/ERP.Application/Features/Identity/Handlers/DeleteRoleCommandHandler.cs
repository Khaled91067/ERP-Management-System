namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed class DeleteRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null) return false;
        if (await roleRepository.HasUsersAsync(role.Id, cancellationToken))
            throw new InvalidOperationException("A role assigned to users cannot be deleted.");

        roleRepository.Delete(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

