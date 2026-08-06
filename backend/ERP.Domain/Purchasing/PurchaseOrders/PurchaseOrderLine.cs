namespace ERP.Domain.Purchasing.PurchaseOrders;

using ERP.Domain.Catalog.Products;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class PurchaseOrderLine : BaseEntity
{
    public int PurchaseOrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitCost { get; private set; } = null!;

    public PurchaseOrder? PurchaseOrder { get; private set; }
    public Product? Product { get; private set; }

    private PurchaseOrderLine() { }

    internal PurchaseOrderLine(
        int productId,
        int quantity,
        decimal unitCost)
    {
        if (productId <= 0)
            throw new BusinessRuleValidationException(
                "Product id must be valid.");

        if (quantity <= 0)
            throw new BusinessRuleValidationException(
                "Quantity must be greater than zero.");

        ProductId = productId;
        Quantity = quantity;
        UnitCost = new Money(unitCost);
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleValidationException(
                "Quantity must be greater than zero.");

        Quantity = quantity;
    }

    internal void ChangeUnitCost(decimal unitCost)
    {
        UnitCost = new Money(unitCost);
    }
}