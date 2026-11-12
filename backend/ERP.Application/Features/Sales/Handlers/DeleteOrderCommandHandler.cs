namespace ERP.Application.Features.Sales.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Application.Features.Sales.Commands;
using ERP.Domain.Catalog.Products;

using MediatR;

public sealed class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IGenericRepository<Product> productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (order is null)
            return false;

        order.EnsureCanBeDeleted();

        foreach (var line in order.OrderLines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product is not null)
            {
                product.IncreaseStock(line.Quantity);
                _productRepository.Update(product);
            }
        }

        order.Cancel();
        _orderRepository.Delete(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.Sales.OrderById(order.Id), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.Sales.OrdersPrefix(), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.Catalog.ProductsPrefix(), cancellationToken);
        foreach (var line in order.OrderLines)
        {
            await _cacheService.RemoveAsync(CacheKeys.Catalog.ProductById(line.ProductId), cancellationToken);
        }

        return true;
    }
}
