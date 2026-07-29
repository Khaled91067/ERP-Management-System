
namespace ERP.Application.Features.Identity.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.DTOs;

using MediatR;

public sealed record GetRolesQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<RoleDto>>;
public sealed record GetRoleByIdQuery(int Id) : IRequest<RoleDto?>;

