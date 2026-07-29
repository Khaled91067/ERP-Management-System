
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record UpdateRoleCommand(int Id, string Name, string Permissions) : IRequest<bool>;

