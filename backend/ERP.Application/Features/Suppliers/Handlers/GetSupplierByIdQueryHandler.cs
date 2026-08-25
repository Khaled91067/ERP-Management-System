
namespace ERP.Application.Features.Suppliers.Handlers;

using ERP.Application.Common.Caching;
using Microsoft.Extensions.Options;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Suppliers.DTOs;
using ERP.Application.Features.Suppliers.Queries;

using MediatR;

public sealed class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetSupplierByIdQueryHandler(
        ISupplierRepository supplierRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _supplierRepository = supplierRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Purchasing.SupplierById(request.Id);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.FrequentDataExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var supplier = await _supplierRepository.GetByIdAsync(request.Id, ct);

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


