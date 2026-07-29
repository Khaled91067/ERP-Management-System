
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record DeleteRoleCommand(int Id) : IRequest<bool>;

