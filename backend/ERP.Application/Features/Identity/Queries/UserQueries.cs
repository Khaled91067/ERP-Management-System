
namespace ERP.Application.Features.Identity.Queries;

using ERP.Application.Common.Models;
using ERP.Application.Features.Identity.DTOs;

using MediatR;

public sealed record GetUsersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<UserDto>>;
public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
