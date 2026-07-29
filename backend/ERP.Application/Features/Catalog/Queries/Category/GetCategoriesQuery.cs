
namespace ERP.Application.Features.Catalog.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Catalog.DTOs;

using MediatR;

public sealed record GetCategoriesQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<CategoryDto>>;
