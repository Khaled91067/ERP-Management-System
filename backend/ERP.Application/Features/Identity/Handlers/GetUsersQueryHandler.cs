namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Users;

using MediatR;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<User>();
        options.Includes.Add(u => u.Role);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            options.Filter = u => u.FirstName.ToLower().Contains(search) ||
                                  u.LastName.ToLower().Contains(search) ||
                                  u.Email.Value.ToLower().Contains(search);
        }

        var pagedUsers = await userRepository.GetPagedAsync(options, request.Page, request.PageSize);
        return pagedUsers.Map(ToDto);
    }

    private static UserDto ToDto(User user) => new(user.Id, user.FirstName, user.LastName, user.Email.Value,
        user.RoleId, user.Role?.Name ?? string.Empty);
}

