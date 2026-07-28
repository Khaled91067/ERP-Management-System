
namespace ERP.Application.Features.PurchaseOrders.Commands.Models;

using MediatR;

public sealed record ReceivePurchaseOrderCommand(int PurchaseOrderId) : IRequest;