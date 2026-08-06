
namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public AdjustStockCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            return false;

        if (request.QuantityChange > 0)
            product.IncreaseStock(request.QuantityChange);
        else if (request.QuantityChange < 0)
            product.DecreaseStock(Math.Abs(request.QuantityChange));

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"Catalog:Product:{product.Id}", cancellationToken);
        await _cacheService.RemoveByPrefixAsync("Catalog:Products", cancellationToken);

        return true;
    }
}

