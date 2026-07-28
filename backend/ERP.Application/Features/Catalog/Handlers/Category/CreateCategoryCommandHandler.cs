namespace ERP.Application.Features.Catalog.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using ERP.Domain.Catalog.Categories;

using MediatR;

public sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await categoryRepository.GetByNameAsync(name, cancellationToken) is not null)
            throw new InvalidOperationException("A category with this name already exists.");

        var category = new Category(request.Name);
        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}