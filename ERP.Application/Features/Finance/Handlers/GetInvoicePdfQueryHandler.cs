using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Queries.Models;
using ERP.Domain.Exceptions;
using MediatR;
using System.Text;

namespace ERP.Application.Features.Finance.Handlers;

public sealed class GetInvoicePdfQueryHandler : IRequestHandler<GetInvoicePdfQuery, byte[]>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicePdfQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<byte[]> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (invoice is null)
            throw new DomainException($"Invoice with ID {request.Id} was not found.");

        // In a real application, you would use a library like iText7, DinkToPdf, or QuestPDF to generate a PDF.
        // For this example, we're returning a dummy text file encoded as bytes.
        var sb = new StringBuilder();
        sb.AppendLine($"INVOICE #{invoice.Id}");
        sb.AppendLine($"Date: {invoice.InvoiceDate.ToShortDateString()}");
        sb.AppendLine($"Due Date: {invoice.DueDate.ToShortDateString()}");
        sb.AppendLine($"Status: {invoice.Status}");
        sb.AppendLine(new string('-', 30));
        
        foreach (var line in invoice.InvoiceLines)
        {
            sb.AppendLine($"{line.Description} | Qty: {line.Quantity} | Price: {line.UnitPrice:C} | Total: {line.Quantity * line.UnitPrice:C}");
        }
        
        sb.AppendLine(new string('-', 30));
        sb.AppendLine($"Total Amount: {invoice.TotalAmount:C}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
