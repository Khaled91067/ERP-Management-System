
namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.DTOs;
using global::ERP.Application.Features.Catalog.Queries;

using MediatR;

using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Common.Caching;
using global::Microsoft.Extensions.Options;

public sealed class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    ICacheService cacheService,
    IOptions<CacheSettings> cacheSettings)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Catalog:Product:{request.Id}";
        var expiration = TimeSpan.FromMinutes(cacheSettings.Value.FrequentDataExpirationMinutes);

        return await cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var product = await productRepository.GetByIdWithCategoryAsync(request.Id, cancellationToken);
                return product is null ? null : new ProductDto(product.Id, product.Name, product.Sku,
                    product.CategoryId, product.Category?.Name ?? string.Empty, product.UnitPrice.Amount, product.CostPrice.Amount,
                    product.StockQuantity, product.ReorderLevel);
            },
            expiration,
            true,
            cancellationToken);
    }
}
