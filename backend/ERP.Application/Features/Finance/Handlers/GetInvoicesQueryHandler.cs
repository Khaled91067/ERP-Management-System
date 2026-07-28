namespace ERP.Application.Features.Finance.Handlers;

using System;
using System.Collections.Generic;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.Finance.Dtos;
using ERP.Application.Features.Finance.Queries.Models;
using ERP.Domain.Sales.Invoices;

using MediatR;

public sealed class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<PagedResult<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
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
    }
}
