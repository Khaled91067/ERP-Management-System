using ERP.Application.Features.PurchaseOrders.Dtos;
using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Queries.Models;

public sealed record GetPurchaseOrderByIdQuery(int Id): IRequest<PurchaseOrderDto>;