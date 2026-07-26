using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request.Sku, request.CategoryId, request.UnitPrice, request.CostPrice,
            request.InitialStockQuantity, request.ReorderLevel, cancellationToken);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Sku = request.Sku.Trim(),
            CategoryId = request.CategoryId,
            UnitPrice = request.UnitPrice,
            CostPrice = request.CostPrice,
            ReorderLevel = request.ReorderLevel
        };
        if (request.InitialStockQuantity > 0)
            product.IncreaseStock(request.InitialStockQuantity);

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
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