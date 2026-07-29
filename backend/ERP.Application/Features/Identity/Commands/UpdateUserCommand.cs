
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record UpdateUserCommand(int Id, string FirstName, string LastName, string Email) : IRequest<bool>;

