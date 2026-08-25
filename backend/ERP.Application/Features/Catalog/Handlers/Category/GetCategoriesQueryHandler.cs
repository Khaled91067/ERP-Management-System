
namespace ERP.Application.Features.Catalog.Handlers;

using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Application.Common.Models;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Catalog.Categories;
using Microsoft.Extensions.Options;

using MediatR;

public sealed class GetCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IOptions<CacheSettings> cacheSettings)
    : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Catalog:Categories:Search={request.Search?.Trim() ?? "none"}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(cacheSettings.Value.ReferenceDataExpirationMinutes);

        return await cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Category> 
                { 
                    AsNoTracking = true,
                    OrderBy = q => Queryable.OrderBy(q, c => c.Name)
                };

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(c => c.Name.Contains(search));
                }

                var pagedCategories = await categoryRepository.GetPagedAsync(options, request.Page, request.PageSize, ct);

                return pagedCategories.Map(category => new CategoryDto(
                    category.Id,
                    category.Name));
            },
            expiration,
            false,
            cancellationToken);
    }
}
