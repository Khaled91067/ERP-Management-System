using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Application.Features.PurchaseOrders.Commands.Models;

public sealed record UpdatePurchaseOrderCommand(
    int Id,
    int SupplierId,
    DateTime ExpectedDelivery,
    List<UpdatePurchaseOrderLineCommand> Lines
) : IRequest<bool>;

public sealed record UpdatePurchaseOrderLineCommand(
    int ProductId,
    int Quantity,
    decimal UnitCost
);
