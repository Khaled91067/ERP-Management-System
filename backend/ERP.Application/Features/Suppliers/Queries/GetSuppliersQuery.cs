using ERP.Application.Common.Models;
using ERP.Application.Features.Suppliers.DTOs;
using MediatR;

namespace ERP.Application.Features.Suppliers.Queries;

public sealed record GetSuppliersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<SupplierDto>>;
