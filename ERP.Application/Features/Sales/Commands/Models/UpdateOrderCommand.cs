using ERP.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace ERP.Application.Features.Sales.Commands.Models;

public sealed record UpdateOrderCommand(
    int Id,
    PaymentMethod PaymentMethod,
    string ShippingAddress,
    List<UpdateOrderLineCommand> Lines
) : IRequest<bool>;

public sealed record UpdateOrderLineCommand(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage = 0
);
