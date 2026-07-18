using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Commands.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.PurchaseOrders.Handlers;

public sealed class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, bool>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
            return false;

        // 1. Clear old lines (recalculates total automatically)
        purchaseOrder.ClearLines();

        // 2. Update expected delivery & supplier
        purchaseOrder.UpdateDetails(request.SupplierId, request.ExpectedDelivery);

        // 3. Add new lines
        foreach (var line in request.Lines)
        {
            purchaseOrder.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitCost);
        }

        _purchaseOrderRepository.Update(purchaseOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
