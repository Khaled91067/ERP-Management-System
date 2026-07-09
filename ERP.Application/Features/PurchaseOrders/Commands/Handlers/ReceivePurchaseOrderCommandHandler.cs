using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Commands.Models;
using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Commands.Handlers;

public sealed class ReceivePurchaseOrderCommandHandler
    : IRequestHandler<ReceivePurchaseOrderCommand>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceivePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ReceivePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder =
            await _purchaseOrderRepository.GetByIdWithLinesAsync(
                request.PurchaseOrderId,
                cancellationToken);

        if (purchaseOrder is null)
            throw new InvalidOperationException(
                "Purchase order was not found.");

        purchaseOrder.Receive();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}