using MediatR;

namespace ERP.Application.Features.Authentication.Commands.Models;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<int>;