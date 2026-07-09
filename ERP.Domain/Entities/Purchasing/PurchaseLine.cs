using ERP.Domain.Exceptions;

namespace ERP.Domain.Entities;

public class PurchaseLine
{
    public int Id { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }

    public PurchaseOrder? PurchaseOrder { get; private set; }
    public Product? Product { get; private set; }

    private PurchaseLine() { }

    internal PurchaseLine(
        int productId,
        int quantity,
        decimal unitCost)
    {
        if (productId <= 0)
            throw new DomainException(
                "Product id must be valid.");

        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        if (unitCost < 0)
            throw new DomainException(
                "Unit cost cannot be negative.");

        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        Quantity = quantity;
    }

    internal void ChangeUnitCost(decimal unitCost)
    {
        if (unitCost < 0)
            throw new DomainException(
                "Unit cost cannot be negative.");

        UnitCost = unitCost;
    }
}