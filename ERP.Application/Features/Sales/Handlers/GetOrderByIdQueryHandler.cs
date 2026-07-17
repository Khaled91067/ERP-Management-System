using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.Sales.Handlers;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (order is null)
            return null;

        var linesDto = order.OrderLines.Select(ol => new OrderLineDto(
            ol.Id,
            ol.ProductId,
            ol.Product?.Name ?? string.Empty,
            ol.Quantity,
            ol.UnitPrice,
            ol.DiscountPercentage,
            (ol.Quantity * ol.UnitPrice) * (1 - ol.DiscountPercentage / 100)
        )).ToList();

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.Customer?.Name ?? string.Empty,
            order.OrderDate,
            order.Status.ToString(),
            order.PaymentMethod.ToString(),
            order.ShippingAddress,
            order.TotalAmount,
            linesDto);
    }
}
