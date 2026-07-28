namespace ERP.Application.Features.Finance.Handlers;

using System.Linq;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Finance.Queries.Models;
using ERP.Domain.Shared.Exceptions;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            throw new NotFoundException("Invoice", request.Id);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"INVOICE #{invoice.Id:D5}").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                        row.RelativeItem().AlignRight().Text("ERP Management System").FontSize(16).SemiBold();
                    });

                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Bill To:").SemiBold();
                            if (invoice.Customer != null)
                            {
                                c.Item().Text(invoice.Customer.Name);
                                c.Item().Text(invoice.Customer.Address);
                                c.Item().Text($"{invoice.Customer.City} {invoice.Customer.Country}");
                                c.Item().Text(invoice.Customer.Email.Value);
                                c.Item().Text(invoice.Customer.Phone);
                            }
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text($"Date: {invoice.InvoiceDate:d}");
                            c.Item().Text($"Due Date: {invoice.DueDate:d}");
                            c.Item().Text($"Status: {invoice.Status}");
                            if (invoice.OrderId > 0)
                            {
                                c.Item().Text($"Order Ref: ORD-{invoice.OrderId:D5}");
                            }
                        });
                    });
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Description").SemiBold();
                            header.Cell().AlignRight().Text("Unit Price").SemiBold();
                            header.Cell().AlignRight().Text("Qty").SemiBold();
                            header.Cell().AlignRight().Text("Tax").SemiBold();
                            header.Cell().AlignRight().Text("Total").SemiBold();
                            
                            header.Cell().ColumnSpan(5).PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                        foreach (var line in invoice.InvoiceLines)
                        {
                            table.Cell().Text(line.Description);
                            table.Cell().AlignRight().Text($"{line.UnitPrice.Amount:C}");
                            table.Cell().AlignRight().Text($"{line.Quantity}");
                            table.Cell().AlignRight().Text($"{line.TaxRate}%");
                            
                            var lineTotal = (line.Quantity * line.UnitPrice.Amount) * (1 + line.TaxRate / 100);
                            table.Cell().AlignRight().Text($"{lineTotal:C}");
                        }
                    });

                    col.Item().PaddingTop(20).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Total Amount: {invoice.TotalAmount.Amount:C}").FontSize(14).SemiBold();
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Thank you for your business. ");
                    t.Span($"Generated on {DateTime.UtcNow:g}").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        });

        return document.GeneratePdf();
    }
}
