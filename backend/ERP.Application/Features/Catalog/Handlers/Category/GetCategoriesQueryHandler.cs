using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
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
    }
}