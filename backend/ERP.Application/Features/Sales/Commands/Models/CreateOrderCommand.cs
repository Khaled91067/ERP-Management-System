
namespace ERP.Application.Features.Sales.Commands.Models;

using System.Collections.Generic;

using ERP.Domain.Sales.Orders;

using MediatR;

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
