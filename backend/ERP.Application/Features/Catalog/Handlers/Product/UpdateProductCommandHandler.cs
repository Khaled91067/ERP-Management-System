namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.Commands;

using MediatR;

using global::ERP.Application.Abstractions.Caching;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService) : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null) return false;
        if (request.UnitPrice < 0 || request.CostPrice < 0 || request.ReorderLevel < 0)
            throw new InvalidOperationException("Prices and reorder level cannot be negative.");
        if (await categoryRepository.GetByIdAsync(request.CategoryId) is null)
            throw new InvalidOperationException("Category does not exist.");

        var existing = await productRepository.GetBySkuAsync(request.Sku.Trim(), cancellationToken);
        if (existing is not null && existing.Id != product.Id)
            throw new InvalidOperationException("A product with this SKU already exists.");

        product.UpdateDetails(
            request.Name,
            request.Sku,
            request.CategoryId,
            request.UnitPrice,
            request.CostPrice,
            request.ReorderLevel);

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await cacheService.RemoveAsync($"Catalog:Product:{product.Id}", cancellationToken);
        await cacheService.RemoveByPrefixAsync("Catalog:Products", cancellationToken);
        
        return true;
    }
}
