namespace ERP.Application.Features.PurchaseOrders.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Commands;

using MediatR;

public sealed class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, bool>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeletePurchaseOrderCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
            return false;

        purchaseOrder.EnsureCanBeDeleted();

        _repository.Delete(purchaseOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"PO:PurchaseOrder:{purchaseOrder.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("PO:PurchaseOrders", cancellationToken);

        return true;
    }
}
