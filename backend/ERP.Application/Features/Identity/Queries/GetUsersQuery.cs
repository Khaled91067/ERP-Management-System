
namespace ERP.Application.Features.Identity.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.DTOs;

using MediatR;

public sealed record GetUsersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<UserDto>>;

