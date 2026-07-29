
namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Catalog.DTOs;
using global::ERP.Application.Features.Catalog.Queries;
using global::ERP.Domain.Catalog.Categories;

using MediatR;

using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Common.Caching;
using global::Microsoft.Extensions.Options;

public sealed class GetCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IOptions<CacheSettings> cacheSettings)
    : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Catalog:Categories:Search={request.Search?.Trim().ToLower() ?? "none"}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(cacheSettings.Value.ReferenceDataExpirationMinutes);

        return await cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Category>();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    options.Filter = c => c.Name.ToLower().Contains(search);
                }

                var pagedCategories = await categoryRepository.GetPagedAsync(options, request.Page, request.PageSize);

                return pagedCategories.Map(category => new CategoryDto(
                    category.Id,
                    category.Name));
            },
            expiration,
            false,
            cancellationToken);
    }
}
