namespace ERP.Application.Features.Catalog.Handlers;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Application.Common.Models;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Catalog.Products;
using MediatR;
using Microsoft.Extensions.Options;

public sealed class GetProductsQueryHandler(
    IProductRepository productRepository,
    ICacheService cacheService,
    IOptions<CacheSettings> cacheSettings)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var searchPart = request.Search?.Trim() ?? "none";
        var categoryPart = request.CategoryId.HasValue ? request.CategoryId.Value.ToString() : "all";
        var cacheKey = CacheKeys.Catalog.ProductsList(searchPart, categoryPart, page, pageSize);
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

                var pagedProducts = await productRepository.GetPagedAsync(options, page, pageSize, ct);
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
