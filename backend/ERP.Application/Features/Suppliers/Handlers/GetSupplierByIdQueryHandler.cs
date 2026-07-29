
namespace ERP.Application.Features.Suppliers.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Suppliers.DTOs;
using global::ERP.Application.Features.Suppliers.Queries;

using MediatR;

public sealed class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetSupplierByIdQueryHandler(
        ISupplierRepository supplierRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _supplierRepository = supplierRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = global::ERP.Application.Common.Caching.CacheKeys.Purchasing.SupplierById(request.Id);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.FrequentDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var supplier = await _supplierRepository.GetByIdAsync(request.Id);

                if (supplier is null)
                    return null;

                return new SupplierDto(
                    supplier.Id,
                    supplier.CompanyName,
                    supplier.ContactName,
                    supplier.Email.Value,
                    supplier.Phone,
                    supplier.PaymentTerms);
            },
            expiration,
            true,
            cancellationToken);
    }
}


