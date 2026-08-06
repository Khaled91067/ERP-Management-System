namespace ERP.Domain.Sales.Orders;

using ERP.Domain.Catalog.Products;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class OrderLine : BaseEntity
{
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public decimal DiscountPercentage { get; private set; }

    public Order? Order { get; private set; }
    public Product? Product { get; private set; }

    private OrderLine()
    {
    }

    internal OrderLine(
        int productId,
        int quantity,
        decimal unitPrice,
        decimal discountPercentage = 0)
    {
        if (productId <= 0)
            throw new BusinessRuleValidationException("Product id must be valid.");

        if (quantity <= 0)
            throw new BusinessRuleValidationException("Quantity must be greater than zero.");

        if (discountPercentage < 0 || discountPercentage > 100)
            throw new BusinessRuleValidationException(
                "Discount percentage must be between 0 and 100.");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = new Money(unitPrice);
        DiscountPercentage = discountPercentage;
    }
}
