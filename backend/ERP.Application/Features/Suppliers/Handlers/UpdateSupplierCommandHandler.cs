namespace ERP.Application.Features.Suppliers.Handlers;

using ERP.Application.Common.Caching;
using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;

using MediatR;

public sealed class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, bool>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

    public async Task<bool> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            return false;

        supplier.UpdateDetails(
            request.CompanyName,
            request.Email,
            request.ContactName,
            request.Phone,
            request.PaymentTerms);

        _supplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKeys.Purchasing.SupplierById(supplier.Id), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.Purchasing.SuppliersPrefix(), cancellationToken);

        return true;
    }
}


