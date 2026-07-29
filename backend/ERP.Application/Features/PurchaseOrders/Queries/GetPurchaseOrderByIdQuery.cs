
namespace ERP.Application.Features.PurchaseOrders.Queries;

using global::ERP.Application.Features.PurchaseOrders.Dtos;

using MediatR;

public sealed record GetPurchaseOrderByIdQuery(int Id): IRequest<PurchaseOrderDto>;
