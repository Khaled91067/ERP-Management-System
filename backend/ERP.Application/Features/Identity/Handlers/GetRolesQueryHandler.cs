namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Identity.DTOs;
using ERP.Application.Features.Identity.Queries;
using ERP.Domain.Identity.Roles;

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
            var search = request.Search.Trim();
            options.Filters.Add(r => r.Name.Contains(search));
        }

        var pagedRoles = await roleRepository.GetPagedAsync(options, request.Page, request.PageSize, cancellationToken);
        return pagedRoles.Map(role => new RoleDto(role.Id, role.Name, role.Permissions));
    }
}

