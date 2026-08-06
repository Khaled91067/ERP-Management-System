
namespace ERP.Application.Features.Catalog.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Catalog.Commands;

using MediatR;

public sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return false;
        if (await productRepository.HasTransactionsAsync(product.Id, cancellationToken))
            throw new InvalidOperationException("A product with transactions cannot be deleted.");

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await cacheService.RemoveAsync($"Catalog:Product:{product.Id}", cancellationToken);
        await cacheService.RemoveByPrefixAsync("Catalog:Products", cancellationToken);
        
        return true;
    }
}
