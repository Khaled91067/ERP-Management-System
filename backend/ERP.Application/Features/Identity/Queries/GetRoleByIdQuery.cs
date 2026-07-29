
namespace ERP.Application.Features.Identity.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.DTOs;

using MediatR;

public sealed record GetRoleByIdQuery(int Id) : IRequest<RoleDto?>;

