
namespace ERP.Application.Features.Sales.Handlers;

using System;
using System.Collections.Generic;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries.Models;
using ERP.Domain.Sales.Orders;

using MediatR;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Order>();
        options.Includes.Add(o => o.Customer);

        if (request.CustomerId.HasValue)
        {
            options.Filter = o => o.CustomerId == request.CustomerId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var statusEnum))
        {
            var existingFilter = options.Filter;
            options.Filter = o => (existingFilter == null || existingFilter.Compile()(o)) && o.Status == statusEnum;
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            var existingFilter = options.Filter;
            options.Filter = o => (existingFilter == null || existingFilter.Compile()(o)) &&
                                  ((o.Customer != null && o.Customer.Name.ToLower().Contains(search)) ||
                                   o.ShippingAddress.ToLower().Contains(search));
        }

        var pagedOrders = await _orderRepository.GetPagedAsync(options, request.Page, request.PageSize);

        return pagedOrders.Map(order => new OrderDto(
            order.Id,
            order.CustomerId,
            order.Customer?.Name ?? string.Empty,
            order.OrderDate,
            order.Status.ToString(),
            order.PaymentMethod.ToString(),
            order.ShippingAddress,
            order.TotalAmount,
            new List<OrderLineDto>()
        ));
    }
}
