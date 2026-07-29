
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record ChangeUserRoleCommand(int Id, int RoleId) : IRequest<bool>;

