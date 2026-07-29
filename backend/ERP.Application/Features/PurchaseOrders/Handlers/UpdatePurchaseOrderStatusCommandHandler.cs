
namespace ERP.Application.Features.PurchaseOrders.Handlers;


using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.PurchaseOrders.Commands.Models;
using global::ERP.Domain.Purchasing.PurchaseOrders;
using global::ERP.Domain.Shared.Exceptions;

using MediatR;

public sealed class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, bool>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public UpdatePurchaseOrderStatusCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _repository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (purchaseOrder is null)
            return false;

        if (!Enum.TryParse<PurchaseOrderStatus>(request.Status, true, out var newStatus))
            throw new BusinessRuleValidationException($"Invalid status: {request.Status}. Valid values: Draft, Submitted, Approved, Received, Cancelled.");

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
                throw new BusinessRuleValidationException($"Cannot transition to status: {newStatus}");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"PO:PurchaseOrder:{purchaseOrder.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("PO:PurchaseOrders", cancellationToken);

        return true;
    }
}


