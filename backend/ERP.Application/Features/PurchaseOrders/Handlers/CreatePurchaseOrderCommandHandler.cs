namespace ERP.Application.Features.PurchaseOrders.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.PurchaseOrders.Commands.Models;
using global::ERP.Domain.Purchasing.PurchaseOrders;

using MediatR;

public sealed class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, int>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public CreatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<int> Handle(
        CreatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = new PurchaseOrder(
            request.SupplierId,
            request.ExpectedDelivery);

        foreach (var line in request.Lines)
        {
            purchaseOrder.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitCost);
        }

        _purchaseOrderRepository.Add(purchaseOrder);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync("PO:PurchaseOrders", cancellationToken);

        return purchaseOrder.Id;
    }
}

