using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.DTOs;
using ERP.Application.Features.Catalog.Queries;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        return category is null ? null : new CategoryDto(category.Id, category.Name);
    }
}