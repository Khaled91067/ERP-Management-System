
namespace ERP.Application.Features.Identity.Queries;

using ERP.Application.Features.Identity.DTOs;

using MediatR;

public sealed record GetRoleByIdQuery(int Id) : IRequest<RoleDto?>;

