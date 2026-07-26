using MediatR;

namespace ERP.Application.Features.Identity.Commands;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    int RoleId) : IRequest<int>;

public sealed record UpdateUserCommand(int Id, string FirstName, string LastName, string Email) : IRequest<bool>;
public sealed record ChangeUserRoleCommand(int Id, int RoleId) : IRequest<bool>;
public sealed record DeleteUserCommand(int Id) : IRequest<bool>;
