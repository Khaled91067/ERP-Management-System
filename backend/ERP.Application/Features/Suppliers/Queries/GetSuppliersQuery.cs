
namespace ERP.Application.Features.Suppliers.Queries;

using ERP.Application.Common.Models;
using ERP.Application.Features.Suppliers.DTOs;

using MediatR;

public sealed record GetSuppliersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<SupplierDto>>;
