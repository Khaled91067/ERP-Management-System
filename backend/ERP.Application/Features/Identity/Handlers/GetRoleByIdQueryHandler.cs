namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Roles;

using MediatR;

public sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRoleByIdQuery, RoleDto?>
{
    public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        return role is null ? null : new RoleDto(role.Id, role.Name, role.Permissions);
    }
}

