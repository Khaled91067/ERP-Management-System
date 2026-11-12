namespace ERP.Application.Features.PurchaseOrders.Handlers;

using System.Linq;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Dtos;
using ERP.Application.Features.PurchaseOrders.Queries;

using MediatR;

public sealed class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto?>
{
    private readonly IPurchaseOrderRepository _repository;

    public GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PurchaseOrderDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder =
            await _repository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (purchaseOrder is null)
            return null;

        return new PurchaseOrderDto(
            purchaseOrder.Id,
            purchaseOrder.SupplierId,
            purchaseOrder.Supplier?.CompanyName ?? string.Empty,
            purchaseOrder.OrderDate,
            purchaseOrder.ExpectedDelivery,
            purchaseOrder.Status.ToString(),
            purchaseOrder.TotalAmount.Amount,
            purchaseOrder.PurchaseOrderLines.Select(l => new PurchaseOrderLineDto(
                l.Id,
                l.ProductId,
                l.Product?.Name ?? string.Empty,
                l.Quantity,
                l.UnitCost.Amount,
                l.Quantity * l.UnitCost.Amount)).ToList());
    }
}
