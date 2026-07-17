using ERP.Application.Features.Identity.DTOs;
using MediatR;

namespace ERP.Application.Features.Identity.Queries;

public sealed record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
