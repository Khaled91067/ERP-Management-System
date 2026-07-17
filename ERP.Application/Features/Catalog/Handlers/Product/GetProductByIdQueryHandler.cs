using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdWithCategoryAsync(request.Id, cancellationToken);
        return product is null ? null : new ProductDto(product.Id, product.Name, product.Sku,
            product.CategoryId, product.Category?.Name ?? string.Empty, product.UnitPrice, product.CostPrice,
            product.StockQuantity, product.ReorderLevel);
    }
}