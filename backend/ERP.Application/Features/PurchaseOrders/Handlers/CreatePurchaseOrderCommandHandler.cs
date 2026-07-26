using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Commands.Models;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Handlers;

public sealed class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, int>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository purchaseOrderRepository, IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
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

        return purchaseOrder.Id;
    }
}