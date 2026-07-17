using MediatR;
using ERP.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ERP.Application.Features.Sales.Commands.Models;

public sealed record CreateOrderCommand(
    int CustomerId,
    PaymentMethod PaymentMethod,
    string ShippingAddress,
    List<CreateOrderLineCommand> Lines
) : IRequest<int>;

public sealed record CreateOrderLineCommand(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage = 0
);
