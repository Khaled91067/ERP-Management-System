namespace ERP.Application.Features.PurchaseOrders.Handlers;

using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Application.Common.Models;
using ERP.Application.Features.PurchaseOrders.Dtos;
using ERP.Application.Features.PurchaseOrders.Queries;
using ERP.Domain.Purchasing.PurchaseOrders;
using MediatR;
using Microsoft.Extensions.Options;

public sealed class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetPurchaseOrdersQueryHandler(
        IPurchaseOrderRepository repository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _repository = repository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var supPart = request.SupplierId.HasValue ? request.SupplierId.Value.ToString() : "all";
        var statPart = !string.IsNullOrWhiteSpace(request.Status) ? request.Status : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";

        var cacheKey = CacheKeys.Purchasing.PurchaseOrdersList(supPart, statPart, searchPart, page, pageSize);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<PurchaseOrder>
                {
                    AsNoTracking = true,
                    OrderBy = q => System.Linq.Queryable.OrderByDescending(q, po => po.OrderDate)
                };
                options.Includes.Add(po => po.Supplier);

                if (request.SupplierId.HasValue)
                {
                    options.Filters.Add(po => po.SupplierId == request.SupplierId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<PurchaseOrderStatus>(request.Status, true, out var statusFilter))
                {
                    options.Filters.Add(po => po.Status == statusFilter);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(po => po.Supplier != null && (po.Supplier.CompanyName.Contains(search) || po.Supplier.ContactName.Contains(search)));
                }

                var pagedOrders = await _repository.GetPagedAsync(options, page, pageSize, ct);

                return pagedOrders.Map(po => new PurchaseOrderDto(
                    po.Id,
                    po.SupplierId,
                    po.Supplier?.CompanyName ?? string.Empty,
                    po.OrderDate,
                    po.ExpectedDelivery,
                    po.Status.ToString(),
                    po.TotalAmount.Amount));
            },
            expiration,
            false,
            cancellationToken);
    }
}
