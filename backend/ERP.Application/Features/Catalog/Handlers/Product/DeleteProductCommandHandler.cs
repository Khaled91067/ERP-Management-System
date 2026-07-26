using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null) return false;
        if (await productRepository.HasTransactionsAsync(product.Id, cancellationToken))
            throw new InvalidOperationException("A product with transactions cannot be deleted.");

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}