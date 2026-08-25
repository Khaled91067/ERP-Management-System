namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;
using ERP.Domain.Identity.Roles;

using MediatR;

public sealed class CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, int>
{
    public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roleRepository.GetByNameAsync(request.Name.Trim(), cancellationToken) is not null)
            throw new InvalidOperationException("A role with this name already exists.");

        var role = new Role(request.Name, request.Permissions);
        roleRepository.Add(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return role.Id;
    }
}

