using ERP.Application.Features.Identity.DTOs;
using MediatR;

namespace ERP.Application.Features.Identity.Queries;

public sealed record GetRolesQuery : IRequest<IReadOnlyList<RoleDto>>;
public sealed record GetRoleByIdQuery(int Id) : IRequest<RoleDto?>;
