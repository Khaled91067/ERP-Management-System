using ERP.Application.Abstractions.Messaging;
using ERP.Application.Abstractions.Repositories;
using ERP.Domain.Entities;
using ERP.Domain.Events;

namespace ERP.Application.Features.PurchaseOrders.EventHandlers;

public sealed class PurchaseOrderReceivedDomainEventHandler: IDomainEventHandler<PurchaseOrderReceivedDomainEvent>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IGenericRepository<Product> _productRepository;

    public PurchaseOrderReceivedDomainEventHandler(IPurchaseOrderRepository purchaseOrderRepository, IGenericRepository<Product> productRepository)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(DomainEventNotification<PurchaseOrderReceivedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var purchaseOrder =
            await _purchaseOrderRepository.GetByIdWithLinesAsync(domainEvent.PurchaseOrderId, cancellationToken);

        if (purchaseOrder is null)
            throw new InvalidOperationException(
                "Purchase order was not found.");

        foreach (var line in purchaseOrder.PurchaseLines)
        {
            var product =
                await _productRepository.GetByIdAsync(line.ProductId);

            if (product is null)
                throw new InvalidOperationException(
                    $"Product with id {line.ProductId} was not found.");

            product.IncreaseStock(line.Quantity);
        }
    }
}