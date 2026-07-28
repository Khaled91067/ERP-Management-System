namespace ERP.Application.Features.Catalog.Handlers;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Catalog.Products;

using MediatR;

public sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Product>();
        options.Includes.Add(p => p.Category);

        if (request.CategoryId.HasValue)
        {
            options.Filter = p => p.CategoryId == request.CategoryId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            var existingFilter = options.Filter;
            options.Filter = p => (existingFilter == null || existingFilter.Compile()(p)) &&
                                  (p.Name.ToLower().Contains(search) || p.Sku.ToLower().Contains(search));
        }

        var pagedProducts = await productRepository.GetPagedAsync(options, request.Page, request.PageSize);
        return pagedProducts.Map(ToDto);
    }

    private static ProductDto ToDto(Product product) => new(product.Id, product.Name, product.Sku,
        product.CategoryId, product.Category?.Name ?? string.Empty, product.UnitPrice.Amount, product.CostPrice.Amount,
        product.StockQuantity, product.ReorderLevel);
}