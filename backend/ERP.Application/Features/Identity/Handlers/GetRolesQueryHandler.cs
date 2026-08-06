namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Roles;

using MediatR;

public sealed class GetRolesQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Role> 
        { 
            AsNoTracking = true,
            OrderBy = q => System.Linq.Queryable.OrderBy(q, r => r.Name)
        };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            options.Filters.Add(r => r.Name.ToLower().Contains(search));
        }

        var pagedRoles = await roleRepository.GetPagedAsync(options, request.Page, request.PageSize);
        return pagedRoles.Map(role => new RoleDto(role.Id, role.Name, role.Permissions));
    }
}

