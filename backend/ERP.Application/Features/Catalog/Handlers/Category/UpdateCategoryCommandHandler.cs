namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IRequestHandler<UpdateCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null) return false;

        var name = request.Name.Trim();
        var existing = await categoryRepository.GetByNameAsync(name, cancellationToken);
        if (existing is not null && existing.Id != category.Id)
            throw new InvalidOperationException("A category with this name already exists.");

        category.UpdateName(request.Name);
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await cacheService.RemoveAsync($"Catalog:Category:{category.Id}", cancellationToken);
        await cacheService.RemoveByPrefixAsync("Catalog:Categories", cancellationToken);
        
        return true;
    }
}
