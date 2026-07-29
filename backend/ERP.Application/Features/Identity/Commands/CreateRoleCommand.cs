
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record CreateRoleCommand(string Name, string Permissions) : IRequest<int>;

