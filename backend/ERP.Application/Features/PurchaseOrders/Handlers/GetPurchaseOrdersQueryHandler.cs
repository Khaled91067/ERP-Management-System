namespace ERP.Application.Features.PurchaseOrders.Handlers;

using System;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.PurchaseOrders.Dtos;
using ERP.Application.Features.PurchaseOrders.Queries.Models;
using ERP.Domain.Purchasing.PurchaseOrders;

using MediatR;

public sealed class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;

    public GetPurchaseOrdersQueryHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
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
    }
}
