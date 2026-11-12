namespace ERP.Infrastructure.Persistence.Seeding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ERP.Domain.Sales.Orders;

using Microsoft.EntityFrameworkCore;

public class OrderSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public OrderSeeder(AppDbContext context)
    {
        _context = context;
    }

    private class OrderLineSeedDto
    {
        public string ProductSku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
    }

    private class OrderSeedDto
    {
        public string CustomerEmail { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CreditCard";
        public string ShippingAddress { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public List<OrderLineSeedDto> Lines { get; set; } = [];
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var dtos = await SeedHelper.ReadSeedDataAsync<OrderSeedDto>("orders.json");
        if (dtos.Count == 0) return;

        var customers = await _context.Customers
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Email.Value, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var products = await _context.Products
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Sku, p => p.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingOrders = await _context.Orders
            .AsNoTracking()
            .Select(o => new { o.CustomerId, o.ShippingAddress })
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(
            existingOrders.Select(o => $"{o.CustomerId}:{o.ShippingAddress}"),
            StringComparer.OrdinalIgnoreCase);

        var addedAny = false;

        foreach (var dto in dtos)
        {
            if (!customers.TryGetValue(dto.CustomerEmail.Trim(), out var customerId))
            {
                continue;
            }

            var key = $"{customerId}:{dto.ShippingAddress.Trim()}";
            if (existingSet.Contains(key))
            {
                continue;
            }

            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod))
            {
                paymentMethod = PaymentMethod.CreditCard;
            }

            var order = new Order(customerId, paymentMethod, dto.ShippingAddress);

            foreach (var lineDto in dto.Lines)
            {
                if (products.TryGetValue(lineDto.ProductSku.Trim(), out var productId) && lineDto.Quantity > 0)
                {
                    order.AddLine(productId, lineDto.Quantity, lineDto.UnitPrice, lineDto.DiscountPercentage);
                }
            }

            if (order.OrderLines.Count == 0)
            {
                continue;
            }

            var targetStatus = dto.Status?.Trim();
            if (string.Equals(targetStatus, "Shipped", StringComparison.OrdinalIgnoreCase))
            {
                order.Ship();
            }
            else if (string.Equals(targetStatus, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                order.Ship();
                order.Deliver();
            }
            else if (string.Equals(targetStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                order.Cancel();
            }

            await _context.Orders.AddAsync(order, cancellationToken);
            existingSet.Add(key);
            addedAny = true;
        }

        if (addedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
