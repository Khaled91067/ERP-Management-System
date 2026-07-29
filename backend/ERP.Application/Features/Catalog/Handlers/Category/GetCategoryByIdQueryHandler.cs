
namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.DTOs;
using global::ERP.Application.Features.Catalog.Queries;

using MediatR;

using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Common.Caching;
using global::Microsoft.Extensions.Options;

public sealed class GetCategoryByIdQueryHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IOptions<CacheSettings> cacheSettings)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Catalog:Category:{request.Id}";
        var expiration = TimeSpan.FromMinutes(cacheSettings.Value.ReferenceDataExpirationMinutes);

        return await cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var category = await categoryRepository.GetByIdAsync(request.Id);
                return category is null ? null : new CategoryDto(category.Id, category.Name);
            },
            expiration,
            true,
            cancellationToken);
    }
}
