using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;
using MediatR;

namespace ERP.Application.Features.Sales.Queries.Models;

public sealed record GetCustomersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<CustomerDto>>;
