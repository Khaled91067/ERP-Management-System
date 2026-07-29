namespace ERP.Application.Features.Suppliers.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Suppliers.DTOs;
using global::ERP.Application.Features.Suppliers.Queries;
using global::ERP.Domain.Purchasing.Suppliers;

using MediatR;

public sealed class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, PagedResult<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetSuppliersQueryHandler(
        ISupplierRepository supplierRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _supplierRepository = supplierRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        var cacheKey = $"Purchasing:Suppliers:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Supplier>();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    options.Filter = s =>
                        s.CompanyName.ToLower().Contains(term) ||
                        s.ContactName.ToLower().Contains(term) ||
                        s.Email.Value.ToLower().Contains(term);
                }

                var pagedSuppliers = await _supplierRepository.GetPagedAsync(options, request.Page, request.PageSize);

                return pagedSuppliers.Map(s => new SupplierDto(
                    s.Id,
                    s.CompanyName,
                    s.ContactName,
                    s.Email.Value,
                    s.Phone,
                    s.PaymentTerms));
            },
            expiration,
            false,
            cancellationToken);
    }
}


