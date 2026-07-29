namespace ERP.Application.Features.PurchaseOrders.Handlers;

using System;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.PurchaseOrders.Dtos;
using global::ERP.Application.Features.PurchaseOrders.Queries;
using global::ERP.Domain.Purchasing.PurchaseOrders;

using MediatR;

public sealed class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetPurchaseOrdersQueryHandler(
        IPurchaseOrderRepository repository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _repository = repository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var supPart = request.SupplierId.HasValue ? request.SupplierId.Value.ToString() : "all";
        var statPart = !string.IsNullOrWhiteSpace(request.Status) ? request.Status : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        
        var cacheKey = $"PO:PurchaseOrders:Sup={supPart}:Stat={statPart}:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<PurchaseOrder>();
                options.Includes.Add(po => po.Supplier);

                if (request.SupplierId.HasValue)
                {
                    options.Filter = po => po.SupplierId == request.SupplierId.Value;
                }

                if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<PurchaseOrderStatus>(request.Status, true, out var statusFilter))
                {
                    var existingFilter = options.Filter;
                    options.Filter = po => (existingFilter == null || existingFilter.Compile()(po)) && po.Status == statusFilter;
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    var existingFilter = options.Filter;
                    options.Filter = po => (existingFilter == null || existingFilter.Compile()(po)) &&
                                           (po.Supplier != null && (po.Supplier.CompanyName.ToLower().Contains(search) || po.Supplier.ContactName.ToLower().Contains(search)));
                }

                var pagedOrders = await _repository.GetPagedAsync(options, request.Page, request.PageSize);

                return pagedOrders.Map(po => new PurchaseOrderDto(
                    po.Id,
                    po.SupplierId,
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


