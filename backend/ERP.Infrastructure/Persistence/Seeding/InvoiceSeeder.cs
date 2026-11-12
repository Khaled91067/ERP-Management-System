namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Sales.Invoices;

using Microsoft.EntityFrameworkCore;

public class InvoiceSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public InvoiceSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class InvoiceLineSeedDto
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
    }

    private class InvoiceSeedDto
    {
        public string CustomerEmail { get; set; } = string.Empty;
        public int DueDateDays { get; set; } = 15;
        public string Status { get; set; } = "Draft";
        public List<InvoiceLineSeedDto> Lines { get; set; } = [];
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await SeedHelper.ReadSeedDataAsync<InvoiceSeedDto>("invoices.json");
        if (dtos.Count == 0) return;

        var customers = await _context.Customers
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Email.Value, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var customerOrders = await _context.Orders
            .AsNoTracking()
            .GroupBy(o => o.CustomerId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(o => o.Id).ToList(), cancellationToken);

        var existingInvoices = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.InvoiceLines)
            .Select(i => new { i.CustomerId, FirstDesc = i.InvoiceLines.Select(l => l.Description).FirstOrDefault() })
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(
            existingInvoices.Where(i => i.FirstDesc != null).Select(i => $"{i.CustomerId}:{i.FirstDesc}"),
            StringComparer.OrdinalIgnoreCase);

        var addedAny = false;

        foreach (var dto in dtos)
        {
            if (!customers.TryGetValue(dto.CustomerEmail.Trim(), out var customerId))
            {
                continue;
            }

            var firstDesc = dto.Lines.FirstOrDefault()?.Description?.Trim() ?? string.Empty;
            var key = $"{customerId}:{firstDesc}";
            if (existingSet.Contains(key))
            {
                continue;
            }

            int? orderId = null;
            if (customerOrders.TryGetValue(customerId, out var orderIds) && orderIds.Count > 0)
            {
                orderId = orderIds.First();
            }

            var dueDate = DateTime.UtcNow.AddDays(dto.DueDateDays > 0 ? dto.DueDateDays : 15);
            var invoice = new Invoice(orderId, customerId, dueDate);

            foreach (var lineDto in dto.Lines)
            {
                if (lineDto.Quantity > 0 && !string.IsNullOrWhiteSpace(lineDto.Description))
                {
                    invoice.AddLine(lineDto.Description.Trim(), lineDto.Quantity, lineDto.UnitPrice, lineDto.TaxRate);
                }
            }

            if (invoice.InvoiceLines.Count == 0)
            {
                continue;
            }

            var targetStatus = dto.Status?.Trim();
            if (string.Equals(targetStatus, "Sent", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Send();
            }
            else if (string.Equals(targetStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Send();
                invoice.Pay();
            }
            else if (string.Equals(targetStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Cancel();
            }

            await _context.Invoices.AddAsync(invoice, cancellationToken);
            existingSet.Add(key);
            addedAny = true;
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
