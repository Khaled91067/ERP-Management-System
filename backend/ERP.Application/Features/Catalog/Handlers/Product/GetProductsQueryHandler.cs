namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Caching;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Catalog.DTOs;
using global::ERP.Application.Features.Catalog.Queries;
using global::ERP.Domain.Catalog.Products;
using global::Microsoft.Extensions.Options;

using MediatR;

public sealed class GetProductsQueryHandler(
    IProductRepository productRepository,
    ICacheService cacheService,
    IOptions<CacheSettings> cacheSettings)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var searchPart = request.Search?.Trim() ?? "none";
        var categoryPart = request.CategoryId.HasValue ? request.CategoryId.Value.ToString() : "all";
        var cacheKey = $"Catalog:Products:Search={searchPart}:Category={categoryPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(cacheSettings.Value.PaginatedListExpirationMinutes);

        return await cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Product> 
                { 
                    AsNoTracking = true,
                    OrderBy = q => Queryable.OrderBy(q, p => p.Name)
                };
                options.Includes.Add(p => p.Category);

                if (request.CategoryId.HasValue)
                {
                    options.Filters.Add(p => p.CategoryId == request.CategoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(p => p.Name.Contains(search) || p.Sku.Contains(search));
                }

                var pagedProducts = await productRepository.GetPagedAsync(options, request.Page, request.PageSize, ct);
                return pagedProducts.Map(ToDto);
            },
            expiration,
            false,
            cancellationToken);
    }

    private static ProductDto ToDto(Product product) => new(product.Id, product.Name, product.Sku,
        product.CategoryId, product.Category?.Name ?? string.Empty, product.UnitPrice.Amount, product.CostPrice.Amount,
        product.StockQuantity, product.ReorderLevel);
}
