using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Dtos;
using ERP.Application.Features.Finance.Queries.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.Finance.Handlers;

public sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);
        if (invoice is null)
            return null;

        var linesDto = invoice.InvoiceLines.Select(il => new InvoiceLineDto(
            il.Id,
            il.Description,
            il.Quantity,
            il.UnitPrice,
            il.TaxRate,
            (il.Quantity * il.UnitPrice) * (1 + il.TaxRate / 100)
        )).ToList();

        return new InvoiceDto(
            invoice.Id,
            invoice.OrderId,
            invoice.CustomerId,
            invoice.Customer?.Name ?? string.Empty,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.Status.ToString(),
            invoice.TotalAmount,
            invoice.PaidAt,
            linesDto);
    }
}

public sealed class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IEnumerable<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetInvoicesByCustomerAsync(request.CustomerId, cancellationToken);

        // Apply Status filter in-memory if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusStr = request.Status.Trim();
            invoices = invoices.Where(x => 
                string.Equals(x.Status.ToString(), statusStr, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        return invoices.Select(invoice => new InvoiceDto(
            invoice.Id,
            invoice.OrderId,
            invoice.CustomerId,
            invoice.Customer?.Name ?? string.Empty,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.Status.ToString(),
            invoice.TotalAmount,
            invoice.PaidAt,
            new List<InvoiceLineDto>() // Keep overview list response lightweight
        )).ToList();
    }
}
