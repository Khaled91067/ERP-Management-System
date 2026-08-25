
namespace ERP.Application.Features.PurchaseOrders.Commands.Handlers;

using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.PurchaseOrders.Commands;

using MediatR;

public sealed class ReceivePurchaseOrderCommandHandler : IRequestHandler<ReceivePurchaseOrderCommand>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceivePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task Handle(ReceivePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder =
            await _purchaseOrderRepository.GetByIdWithLinesAsync(request.PurchaseOrderId, cancellationToken);

        if (purchaseOrder is null)
            throw new InvalidOperationException("Purchase order was not found.");

        purchaseOrder.Receive();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"PO:PurchaseOrder:{purchaseOrder.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("PO:PurchaseOrders", cancellationToken);
    }
}

