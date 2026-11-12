namespace ERP.Application.Features.Catalog.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null) return false;
        if (await categoryRepository.HasProductsAsync(category.Id, cancellationToken))
            throw new BusinessRuleValidationException("A category with products cannot be deleted.");

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"Catalog:Category:{category.Id}", cancellationToken);
        await cacheService.RemoveByPrefixAsync("Catalog:Categories", cancellationToken);

        return true;
    }
}
