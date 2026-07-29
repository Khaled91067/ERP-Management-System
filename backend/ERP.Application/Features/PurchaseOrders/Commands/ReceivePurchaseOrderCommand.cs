
namespace ERP.Application.Features.PurchaseOrders.Commands;

using MediatR;

public sealed record ReceivePurchaseOrderCommand(int PurchaseOrderId) : IRequest;