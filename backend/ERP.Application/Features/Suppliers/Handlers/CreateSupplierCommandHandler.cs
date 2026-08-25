namespace ERP.Application.Features.Suppliers.Handlers;

using ERP.Application.Common.Caching;
using ERP.Application.Abstractions.Caching;
using System.Threading;
using System.Threading.Tasks;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.Commands;
using ERP.Domain.Purchasing.Suppliers;

using MediatR;

public sealed class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private readonly ICacheService _cacheService;

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

        await _cacheService.RemoveByPrefixAsync(CacheKeys.Purchasing.SuppliersPrefix(), cancellationToken);

        return supplier.Id;
    }
}


