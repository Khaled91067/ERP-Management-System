namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.DTOs;
using ERP.Application.Features.Identity.Queries;

using MediatR;

public sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRoleByIdQuery, RoleDto?>
{
    public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        return role is null ? null : new RoleDto(role.Id, role.Name, role.Permissions);
    }
}

