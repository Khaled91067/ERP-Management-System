using ERP.Application.Common.Models;
using ERP.Application.Features.Identity.DTOs;
using MediatR;

namespace ERP.Application.Features.Identity.Queries;

public sealed record GetRolesQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<RoleDto>>;
public sealed record GetRoleByIdQuery(int Id) : IRequest<RoleDto?>;
