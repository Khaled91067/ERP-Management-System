using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllWithCategoriesAsync(cancellationToken);
        return products.Select(ToDto).ToList();
    }

    private static ProductDto ToDto(Product product) => new(product.Id, product.Name, product.Sku,
        product.CategoryId, product.Category?.Name ?? string.Empty, product.UnitPrice, product.CostPrice,
        product.StockQuantity, product.ReorderLevel);
}