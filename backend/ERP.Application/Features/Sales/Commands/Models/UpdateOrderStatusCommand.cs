using MediatR;
using ERP.Domain.Enums;

namespace ERP.Application.Features.Sales.Commands.Models;

public sealed record UpdateOrderStatusCommand(
    int OrderId,
    OrderStatus Status
) : IRequest<bool>;
