namespace ERP.Application.Features.PurchaseOrders.Commands;

using MediatR;

public sealed record DeletePurchaseOrderCommand(int Id) : IRequest<bool>;
