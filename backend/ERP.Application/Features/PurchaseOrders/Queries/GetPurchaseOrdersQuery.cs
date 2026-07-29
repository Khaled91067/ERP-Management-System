
namespace ERP.Application.Features.PurchaseOrders.Queries;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.PurchaseOrders.Dtos;

using MediatR;

public sealed record GetPurchaseOrdersQuery(
    int? SupplierId = null,
    string? Status = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<PurchaseOrderDto>>;

