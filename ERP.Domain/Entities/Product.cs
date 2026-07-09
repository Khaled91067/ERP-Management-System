using ERP.Domain.Entities;
using ERP.Domain.Entities.Orders;
using ERP.Domain.Exceptions;

namespace ERP.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; private set; }
    public int ReorderLevel { get; set; }

    public Category? Category { get; set; }
    public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    public ICollection<PurchaseLine> PurchaseLines { get; set; } = new List<PurchaseLine>();

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "Stock increase quantity must be greater than zero.");

        StockQuantity += quantity;
    }
}
