namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed class UpdateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, bool>
{
    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null) return false;

        var existing = await roleRepository.GetByNameAsync(request.Name.Trim(), cancellationToken);
        if (existing is not null && existing.Id != role.Id)
            throw new InvalidOperationException("A role with this name already exists.");

        role.UpdateDetails(request.Name, request.Permissions);
        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

