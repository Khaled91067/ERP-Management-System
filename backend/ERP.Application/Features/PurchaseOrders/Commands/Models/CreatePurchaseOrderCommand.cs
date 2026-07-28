
namespace ERP.Application.Features.PurchaseOrders.Commands.Models;

using MediatR;

public sealed record CreatePurchaseOrderCommand(
    int SupplierId,
    DateTime ExpectedDelivery,
    List<CreatePurchaseOrderLineCommand> Lines
) : IRequest<int>;

public sealed record CreatePurchaseOrderLineCommand(
    int ProductId,
    int Quantity,
    decimal UnitCost
);