
namespace ERP.Application.Features.Sales.Queries;

using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;

using MediatR;

public sealed record GetOrdersQuery(
    int? CustomerId = null,
    string? Status = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<OrderDto>>;

