
namespace ERP.Application.Features.Suppliers.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Suppliers.DTOs;

using MediatR;

public sealed record GetSuppliersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<SupplierDto>>;

