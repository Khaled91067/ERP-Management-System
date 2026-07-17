using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using ERP.Domain.Entities;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync();

        return categories.Select(category => new CategoryDto(
            category.Id,
            category.Name))
            .ToList();
    }

}