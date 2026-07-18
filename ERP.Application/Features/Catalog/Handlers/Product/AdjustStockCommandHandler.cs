using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Catalog.Commands;
using MediatR;

namespace ERP.Application.Features.Catalog.Handlers;

public sealed class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product is null)
            return false;

        if (request.QuantityChange > 0)
            product.IncreaseStock(request.QuantityChange);
        else if (request.QuantityChange < 0)
            product.DecreaseStock(Math.Abs(request.QuantityChange));

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
