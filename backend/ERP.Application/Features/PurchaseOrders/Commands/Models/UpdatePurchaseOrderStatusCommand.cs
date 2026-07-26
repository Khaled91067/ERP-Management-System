using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Commands.Models;

public sealed record UpdatePurchaseOrderStatusCommand(
    int Id,
    string Status) : IRequest<bool>;
