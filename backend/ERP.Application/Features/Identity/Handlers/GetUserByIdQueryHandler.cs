namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.DTOs;
using ERP.Application.Features.Identity.Queries;

using MediatR;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithRoleAsync(request.Id, cancellationToken);
        return user is null ? null : new UserDto(user.Id, user.FirstName, user.LastName, user.Email.Value,
            user.RoleId, user.Role?.Name ?? string.Empty);
    }
}

