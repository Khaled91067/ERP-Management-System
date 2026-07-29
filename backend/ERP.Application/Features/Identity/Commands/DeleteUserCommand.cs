
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record DeleteUserCommand(int Id) : IRequest<bool>;

