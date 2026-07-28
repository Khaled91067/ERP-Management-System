
namespace ERP.Application.Features.PurchaseOrders.Commands.Models;

using MediatR;

public sealed record UpdatePurchaseOrderStatusCommand(
    int Id,
    string Status) : IRequest<bool>;
