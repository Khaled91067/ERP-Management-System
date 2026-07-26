using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null) return false;

        var name = request.Name.Trim();
        var existing = await categoryRepository.GetByNameAsync(name, cancellationToken);
        if (existing is not null && existing.Id != category.Id)
            throw new InvalidOperationException("A category with this name already exists.");

        category.Name = name;
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}