using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, bool>
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

        product.Name = request.Name.Trim();
        product.Sku = request.Sku.Trim();
        product.CategoryId = request.CategoryId;
        product.UnitPrice = request.UnitPrice;
        product.CostPrice = request.CostPrice;
        product.ReorderLevel = request.ReorderLevel;
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}