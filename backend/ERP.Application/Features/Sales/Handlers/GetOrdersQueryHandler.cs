namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using System;
using System.Collections.Generic;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries;
using ERP.Domain.Sales.Orders;

using MediatR;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetOrdersQueryHandler(
        IOrderRepository orderRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _orderRepository = orderRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var custPart = request.CustomerId.HasValue ? request.CustomerId.Value.ToString() : "all";
        var statPart = !string.IsNullOrWhiteSpace(request.Status) ? request.Status : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";
        
        var cacheKey = CacheKeys.Sales.OrdersList(custPart, statPart, searchPart, request.Page, request.PageSize);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Order> 
                { 
                    AsNoTracking = true,
                    OrderBy = q => System.Linq.Queryable.OrderByDescending(q, o => o.OrderDate)
                };
                options.Includes.Add(o => o.Customer);

                if (request.CustomerId.HasValue)
                {
                    options.Filters.Add(o => o.CustomerId == request.CustomerId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var statusEnum))
                {
                    options.Filters.Add(o => o.Status == statusEnum);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(o => (o.Customer != null && o.Customer.Name.Contains(search)) ||
                                             o.ShippingAddress.Contains(search));
                }

                var pagedOrders = await _orderRepository.GetPagedAsync(options, request.Page, request.PageSize, ct);

                return pagedOrders.Map(order => new OrderDto(
                    order.Id,
                    order.CustomerId,
                    order.Customer?.Name ?? string.Empty,
                    order.OrderDate,
                    order.Status.ToString(),
                    order.PaymentMethod.ToString(),
                    order.ShippingAddress,
                    order.TotalAmount.Amount,
                    new List<OrderLineDto>()
                ));
            },
            expiration,
            false,
            cancellationToken);
    }
}


