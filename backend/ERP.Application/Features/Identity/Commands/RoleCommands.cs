
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record CreateRoleCommand(string Name, string Permissions) : IRequest<int>;
public sealed record UpdateRoleCommand(int Id, string Name, string Permissions) : IRequest<bool>;
public sealed record DeleteRoleCommand(int Id) : IRequest<bool>;
