
namespace ERP.Application.Features.PurchaseOrders.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.PurchaseOrders.Dtos;
using global::ERP.Application.Features.PurchaseOrders.Queries;

using MediatR;

public sealed class GetPurchaseOrderByIdQueryHandler: IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto?>
{
    private readonly IPurchaseOrderRepository _repository;

    public GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PurchaseOrderDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder =
            await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (purchaseOrder is null)
            return null;

        return new PurchaseOrderDto(
            purchaseOrder.Id,
            purchaseOrder.SupplierId,
            purchaseOrder.OrderDate,
            purchaseOrder.ExpectedDelivery,
            purchaseOrder.Status.ToString(),
            purchaseOrder.TotalAmount.Amount);
    }
}
