namespace ERP.Application.Features.PurchaseOrders.EventHandlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Messaging;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Domain.Catalog.Products;
using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Domain.Shared.Exceptions;

public sealed class PurchaseOrderReceivedDomainEventHandler : IDomainEventHandler<PurchaseOrderReceivedDomainEvent>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly ICacheService _cacheService;

    public PurchaseOrderReceivedDomainEventHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IGenericRepository<Product> productRepository,
        ICacheService cacheService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task Handle(DomainEventNotification<PurchaseOrderReceivedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var purchaseOrder =
            await _purchaseOrderRepository.GetByIdWithLinesAsync(domainEvent.PurchaseOrderId, cancellationToken);

        if (purchaseOrder is null)
            throw new NotFoundException("PurchaseOrder", domainEvent.PurchaseOrderId);

        foreach (var line in purchaseOrder.PurchaseOrderLines)
        {
            var product =
                await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);

            if (product is null)
                throw new NotFoundException("Product", line.ProductId);

            product.IncreaseStock(line.Quantity);
            await _cacheService.RemoveAsync(CacheKeys.Catalog.ProductById(line.ProductId), cancellationToken);
        }

        await _cacheService.RemoveByPrefixAsync(CacheKeys.Catalog.ProductsPrefix(), cancellationToken);
    }
}
