
namespace ERP.Application.Features.PurchaseOrders.Commands;

using MediatR;

public sealed record UpdatePurchaseOrderStatusCommand(
    int Id,
    string Status) : IRequest<bool>;
