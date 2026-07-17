using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null) return false;
        if (await categoryRepository.HasProductsAsync(category.Id, cancellationToken))
            throw new InvalidOperationException("A category with products cannot be deleted.");

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}