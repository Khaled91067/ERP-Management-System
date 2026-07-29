namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.Commands;
using global::ERP.Domain.Catalog.Products;

using MediatR;

using global::ERP.Application.Abstractions.Caching;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request.Sku, request.CategoryId, request.UnitPrice, request.CostPrice,
            request.InitialStockQuantity, request.ReorderLevel, cancellationToken);

        var product = new Product(
            request.Name,
            request.Sku,
            request.CategoryId,
            request.UnitPrice,
            request.CostPrice,
            request.ReorderLevel);

        if (request.InitialStockQuantity > 0)
            product.IncreaseStock(request.InitialStockQuantity);

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await cacheService.RemoveByPrefixAsync("Catalog:Products", cancellationToken);
        
        return product.Id;
    }

    private async Task ValidateAsync(string sku, int categoryId, decimal unitPrice, decimal costPrice,
        int initialStockQuantity, int reorderLevel, CancellationToken cancellationToken)
    {
        if (await productRepository.GetBySkuAsync(sku.Trim(), cancellationToken) is not null)
            throw new InvalidOperationException("A product with this SKU already exists.");
        if (await categoryRepository.GetByIdAsync(categoryId) is null)
            throw new InvalidOperationException("Category does not exist.");
        if (unitPrice < 0 || costPrice < 0 || initialStockQuantity < 0 || reorderLevel < 0)
            throw new InvalidOperationException("Prices, stock quantity, and reorder level cannot be negative.");
    }
}
