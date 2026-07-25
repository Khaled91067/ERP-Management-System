using ERP.Application.Common.Models;
using ERP.Application.Features.Catalog.DTOs;
using MediatR;

namespace ERP.Application.Features.Catalog.Queries;

public sealed record GetProductsQuery(
    int? CategoryId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<ProductDto>>;