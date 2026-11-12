namespace ERP.Application.Features.Sales.Handlers;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Application.Common.Models;
using ERP.Application.Features.Sales.Dtos;
using ERP.Application.Features.Sales.Queries;
using ERP.Domain.Sales.Orders;
using MediatR;
using Microsoft.Extensions.Options;

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
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var custPart = request.CustomerId.HasValue ? request.CustomerId.Value.ToString() : "all";
        var statPart = !string.IsNullOrWhiteSpace(request.Status) ? request.Status : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";

        var cacheKey = CacheKeys.Sales.OrdersList(custPart, statPart, searchPart, page, pageSize);
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

                var pagedOrders = await _orderRepository.GetPagedAsync(options, page, pageSize, ct);

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
