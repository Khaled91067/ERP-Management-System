
namespace ERP.Application.Features.Authentication.Commands;

using MediatR;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<int>;