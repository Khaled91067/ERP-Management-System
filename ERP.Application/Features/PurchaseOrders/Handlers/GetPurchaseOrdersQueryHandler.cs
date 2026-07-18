using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Dtos;
using ERP.Application.Features.PurchaseOrders.Queries.Models;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Handlers;

public sealed class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, IEnumerable<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;

    public GetPurchaseOrdersQueryHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<PurchaseOrder>();

        if (request.SupplierId.HasValue)
        {
            var supplierId = request.SupplierId.Value;
            options.Filter = po => po.SupplierId == supplierId;
        }

        var purchaseOrders = await _repository.GetAllAsync(options);

        var result = purchaseOrders.Select(po => new PurchaseOrderDto(
            po.Id,
            po.SupplierId,
            po.OrderDate,
            po.ExpectedDelivery,
            po.Status.ToString(),
            po.TotalAmount));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<PurchaseOrderStatus>(request.Status, true, out var statusFilter))
        {
            result = result.Where(po => po.Status == statusFilter.ToString());
        }

        return result;
    }
}
