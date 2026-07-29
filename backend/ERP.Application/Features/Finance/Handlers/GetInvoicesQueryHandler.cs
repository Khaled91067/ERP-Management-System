namespace ERP.Application.Features.Finance.Handlers;

using System;
using System.Collections.Generic;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Finance.Dtos;
using global::ERP.Application.Features.Finance.Queries.Models;
using global::ERP.Domain.Sales.Invoices;

using MediatR;

using global::ERP.Application.Abstractions.Caching;
using global::ERP.Application.Common.Caching;
using global::Microsoft.Extensions.Options;

public sealed class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetInvoicesQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _invoiceRepository = invoiceRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var custPart = request.CustomerId.HasValue ? request.CustomerId.Value.ToString() : "all";
        var statPart = !string.IsNullOrWhiteSpace(request.Status) ? request.Status : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        
        var cacheKey = $"Finance:Invoices:Cust={custPart}:Stat={statPart}:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Invoice>();
                options.Includes.Add(i => i.Customer);

                if (request.CustomerId.HasValue)
                {
                    options.Filter = i => i.CustomerId == request.CustomerId.Value;
                }

                if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<InvoiceStatus>(request.Status, true, out var statusEnum))
                {
                    var existingFilter = options.Filter;
                    options.Filter = i => (existingFilter == null || existingFilter.Compile()(i)) && i.Status == statusEnum;
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    var existingFilter = options.Filter;
                    options.Filter = i => (existingFilter == null || existingFilter.Compile()(i)) &&
                                          (i.Customer != null && i.Customer.Name.ToLower().Contains(search));
                }

                var pagedInvoices = await _invoiceRepository.GetPagedAsync(options, request.Page, request.PageSize);

                return pagedInvoices.Map(invoice => new InvoiceDto(
                    invoice.Id,
                    invoice.OrderId,
                    invoice.CustomerId,
                    invoice.Customer?.Name ?? string.Empty,
                    invoice.InvoiceDate,
                    invoice.DueDate,
                    invoice.Status.ToString(),
                    invoice.TotalAmount.Amount,
                    invoice.PaidAt,
                    new List<InvoiceLineDto>()
                ));
            },
            expiration,
            false,
            cancellationToken);
    }
}

