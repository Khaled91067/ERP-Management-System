
namespace ERP.Application.Features.Suppliers.Handlers;

using ERP.Application.Common.Caching;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;

using MediatR;

public sealed class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, bool>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task<bool> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            return false;

        _supplierRepository.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.Purchasing.SupplierById(supplier.Id), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.Purchasing.SuppliersPrefix(), cancellationToken);

        return true;
    }
}


