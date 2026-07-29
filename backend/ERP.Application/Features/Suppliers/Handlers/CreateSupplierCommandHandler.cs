namespace ERP.Application.Features.Suppliers.Handlers;

using System.Threading;
using System.Threading.Tasks;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Suppliers.Commands;
using global::ERP.Domain.Purchasing.Suppliers;

using MediatR;

public sealed class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;

    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier(
            request.CompanyName,
            request.Email,
            request.ContactName,
            request.Phone,
            request.PaymentTerms);

        _supplierRepository.Add(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync(global::ERP.Application.Common.Caching.CacheKeys.Purchasing.SuppliersPrefix(), cancellationToken);

        return supplier.Id;
    }
}


