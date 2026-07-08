using ERP.Domain.Exceptions;

namespace ERP.Domain.Entities.Orders;

public class OrderLine
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
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
            throw new DomainException("Product id must be valid.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (discountPercentage < 0 || discountPercentage > 100)
            throw new DomainException(
                "Discount percentage must be between 0 and 100.");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = discountPercentage;
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        Quantity = quantity;
    }

    internal decimal CalculateTotal()
    {
        var subtotal = UnitPrice * Quantity;
        var discount = subtotal * DiscountPercentage / 100;

        return subtotal - discount;
    }


}





