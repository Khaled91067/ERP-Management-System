
namespace ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    int RoleId) : IRequest<int>;

