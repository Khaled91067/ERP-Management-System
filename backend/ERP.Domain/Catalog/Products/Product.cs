
namespace ERP.Domain.Catalog.Products;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using ERP.Domain.Catalog.Categories;
using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Product : SoftDeletableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; private set; }
    public int ReorderLevel { get; set; }
    public Category? Category { get; set; }
    public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleValidationException(
                "Stock increase quantity must be greater than zero.");

        StockQuantity += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleValidationException(
                "Stock decrease quantity must be greater than zero.");

        if (StockQuantity < quantity)
            throw new BusinessRuleValidationException(
                $"Insufficient stock for product {Name}. Available: {StockQuantity}, Requested: {quantity}");

        StockQuantity -= quantity;
    }
}
