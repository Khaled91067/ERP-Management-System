namespace ERP.Application.Features.Catalog.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using ERP.Domain.Catalog.Categories;
using ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await categoryRepository.GetByNameAsync(name, cancellationToken) is not null)
            throw new ConflictException("A category with this name already exists.");

        var category = new Category(request.Name);
        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveByPrefixAsync("Catalog:Categories", cancellationToken);

        return category.Id;
    }
}
