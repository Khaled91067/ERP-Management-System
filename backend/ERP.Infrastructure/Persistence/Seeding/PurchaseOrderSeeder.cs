namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Purchasing.PurchaseOrders;

using Microsoft.EntityFrameworkCore;

public class PurchaseOrderSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public PurchaseOrderSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class PurchaseOrderLineSeedDto
    {
        public string ProductSku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    private class PurchaseOrderSeedDto
    {
        public string SupplierEmail { get; set; } = string.Empty;
        public int ExpectedDeliveryDays { get; set; } = 7;
        public string Status { get; set; } = "Draft";
        public List<PurchaseOrderLineSeedDto> Lines { get; set; } = [];
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await SeedHelper.ReadSeedDataAsync<PurchaseOrderSeedDto>("purchase_orders.json");
        if (dtos.Count == 0) return;

        var suppliers = await _context.Suppliers
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Email.Value, s => s.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var products = await _context.Products
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Sku, p => p.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingPoSupplierIds = await _context.PurchaseOrders
            .AsNoTracking()
            .Select(po => po.SupplierId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var existingPoSupplierSet = new HashSet<int>(existingPoSupplierIds);
        var addedAny = false;

        foreach (var dto in dtos)
        {
            if (!suppliers.TryGetValue(dto.SupplierEmail.Trim(), out var supplierId))
            {
                continue;
            }

            // Idempotency: skip if this supplier already has purchase orders
            if (existingPoSupplierSet.Contains(supplierId))
            {
                continue;
            }

            var expectedDelivery = DateTime.UtcNow.AddDays(dto.ExpectedDeliveryDays > 0 ? dto.ExpectedDeliveryDays : 7);
            var po = new PurchaseOrder(supplierId, expectedDelivery);

            foreach (var lineDto in dto.Lines)
            {
                if (products.TryGetValue(lineDto.ProductSku.Trim(), out var productId) && lineDto.Quantity > 0)
                {
                    po.AddLine(productId, lineDto.Quantity, lineDto.UnitCost);
                }
            }

            if (po.PurchaseOrderLines.Count == 0)
            {
                continue;
            }

            // Apply status transitions according to DDD domain invariants
            var targetStatus = dto.Status?.Trim();
            if (string.Equals(targetStatus, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                po.Submit();
            }
            else if (string.Equals(targetStatus, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                po.Submit();
                po.Approve();
            }
            else if (string.Equals(targetStatus, "Received", StringComparison.OrdinalIgnoreCase))
            {
                po.Submit();
                po.Approve();
                po.Receive();
            }
            else if (string.Equals(targetStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                po.Cancel();
            }

            await _context.PurchaseOrders.AddAsync(po, cancellationToken);
            existingPoSupplierSet.Add(supplierId);
            addedAny = true;
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
