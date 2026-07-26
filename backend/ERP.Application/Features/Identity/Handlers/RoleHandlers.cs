using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Identity.Commands;
using ERP.Application.Features.Identity.DTOs;
using ERP.Application.Features.Identity.Queries;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Identity.Handlers;

public sealed class GetRolesQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Role>();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            options.Filter = r => r.Name.ToLower().Contains(search);
        }

        var pagedRoles = await roleRepository.GetPagedAsync(options, request.Page, request.PageSize);
        return pagedRoles.Map(role => new RoleDto(role.Id, role.Name, role.Permissions));
    }
}

public sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRoleByIdQuery, RoleDto?>
{
    public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        return role is null ? null : new RoleDto(role.Id, role.Name, role.Permissions);
    }
}

public sealed class CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateRoleCommand, int>
{
    public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roleRepository.GetByNameAsync(request.Name.Trim(), cancellationToken) is not null)
            throw new InvalidOperationException("A role with this name already exists.");

        var role = new Role { Name = request.Name.Trim(), Permissions = request.Permissions };
        roleRepository.Add(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return role.Id;
    }
}

public sealed class UpdateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, bool>
{
    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        if (role is null) return false;

        var existing = await roleRepository.GetByNameAsync(request.Name.Trim(), cancellationToken);
        if (existing is not null && existing.Id != role.Id)
            throw new InvalidOperationException("A role with this name already exists.");

        role.Name = request.Name.Trim();
        role.Permissions = request.Permissions;
        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

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
