namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Identity.DTOs;
using ERP.Application.Features.Identity.Queries;
using ERP.Domain.Identity.Users;

using MediatR;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<User> 
        { 
            AsNoTracking = true,
            OrderBy = q => System.Linq.Queryable.ThenBy(System.Linq.Queryable.OrderBy(q, u => u.FirstName), u => u.LastName)
        };
        options.Includes.Add(u => u.Role);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            options.Filters.Add(u => u.FirstName.Contains(search) ||
                                     u.LastName.Contains(search) ||
                                     u.Email.Value.Contains(search));
        }

        var pagedUsers = await userRepository.GetPagedAsync(options, request.Page, request.PageSize, cancellationToken);
        return pagedUsers.Map(ToDto);
    }

    private static UserDto ToDto(User user) => new(user.Id, user.FirstName, user.LastName, user.Email.Value,
        user.RoleId, user.Role?.Name ?? string.Empty);
}

