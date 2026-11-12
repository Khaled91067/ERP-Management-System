namespace ERP.Application.Features.Catalog.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using ERP.Domain.Catalog.Products;
using ERP.Domain.Shared.Exceptions;
using MediatR;

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
            throw new ConflictException("A product with this SKU already exists.");
        if (await categoryRepository.GetByIdAsync(categoryId, cancellationToken) is null)
            throw new NotFoundException("Category", categoryId);
        if (unitPrice < 0 || costPrice < 0 || initialStockQuantity < 0 || reorderLevel < 0)
            throw new BusinessRuleValidationException("Prices, stock quantity, and reorder level cannot be negative.");
    }
}
