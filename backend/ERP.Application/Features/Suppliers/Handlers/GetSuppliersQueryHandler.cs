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
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";
        var cacheKey = global::ERP.Application.Common.Caching.CacheKeys.Purchasing.SuppliersList(searchPart, request.Page, request.PageSize);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Supplier> 
                { 
                    AsNoTracking = true,
                    OrderBy = q => System.Linq.Queryable.OrderBy(q, s => s.CompanyName)
                };

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim();
                    options.Filters.Add(s =>
                        s.CompanyName.Contains(term) ||
                        s.ContactName.Contains(term) ||
                        s.Email.Value.Contains(term));
                }

                var pagedSuppliers = await _supplierRepository.GetPagedAsync(options, request.Page, request.PageSize, ct);

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


