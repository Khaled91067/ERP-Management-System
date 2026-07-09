using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Commands.Models;

public sealed record ReceivePurchaseOrderCommand(int PurchaseOrderId) : IRequest;