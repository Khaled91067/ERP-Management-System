using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Commands.Models;
using ERP.Domain.Enums;
using ERP.Domain.Exceptions;
using MediatR;

namespace ERP.Application.Features.PurchaseOrders.Handlers;

public sealed class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, bool>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseOrderStatusCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _repository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (purchaseOrder is null)
            return false;

        if (!Enum.TryParse<PurchaseOrderStatus>(request.Status, true, out var newStatus))
            throw new DomainException($"Invalid status: {request.Status}. Valid values: Draft, Submitted, Approved, Received, Cancelled.");

        switch (newStatus)
        {
            case PurchaseOrderStatus.Submitted:
                purchaseOrder.Submit();
                break;
            case PurchaseOrderStatus.Approved:
                purchaseOrder.Approve();
                break;
            case PurchaseOrderStatus.Received:
                purchaseOrder.Receive();
                break;
            case PurchaseOrderStatus.Cancelled:
                purchaseOrder.Cancel();
                break;
            default:
                throw new DomainException($"Cannot transition to status: {newStatus}");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
