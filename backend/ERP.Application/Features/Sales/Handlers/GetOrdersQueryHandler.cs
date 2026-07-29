namespace ERP.Application.Features.Sales.Handlers;

using System;
using System.Collections.Generic;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Sales.Dtos;
using global::ERP.Application.Features.Sales.Queries;
using global::ERP.Domain.Sales.Orders;

using MediatR;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetOrdersQueryHandler(
        IOrderRepository orderRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _orderRepository = orderRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var custPart = request.CustomerId.HasValue ? request.CustomerId.Value.ToString() : "all";
        var statPart = !string.IsNullOrWhiteSpace(request.Status) ? request.Status : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        
        var cacheKey = global::ERP.Application.Common.Caching.CacheKeys.Sales.OrdersList(custPart, statPart, searchPart, request.Page, request.PageSize);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
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
                    order.TotalAmount.Amount,
                    new List<OrderLineDto>()
                ));
            },
            expiration,
            false,
            cancellationToken);
    }
}


