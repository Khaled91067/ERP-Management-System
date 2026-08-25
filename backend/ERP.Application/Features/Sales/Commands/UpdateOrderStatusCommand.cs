
namespace ERP.Application.Features.Sales.Commands;

using ERP.Domain.Sales.Orders;

using MediatR;

public sealed record UpdateOrderStatusCommand(
    int OrderId,
    OrderStatus Status
) : IRequest<bool>;

