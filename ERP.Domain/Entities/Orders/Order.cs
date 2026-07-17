using ERP.Domain.Enums;
using ERP.Domain.Exceptions;
using System;
using System.Collections.ObjectModel;

namespace ERP.Domain.Entities.Orders;

public class Order
{
    private readonly List<OrderLine> _orderLines = new();

    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string ShippingAddress { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }

    public Customer? Customer { get; private set; }

    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public ICollection<Invoice> Invoices { get;private set; } = new List<Invoice>();


    private Order()
    {
    }

    public Order(
        int customerId,
        PaymentMethod paymentMethod,
        string shippingAddress)
    {
        if (customerId <= 0)
            throw new DomainException("Customer id must be valid.");

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new DomainException("Shipping address is required.");

        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        ShippingAddress = shippingAddress.Trim();

        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public void AddLine(
        int productId,
        int quantity,
        decimal unitPrice,
        decimal discountPercentage = 0)
    {
        EnsurePending();

        if (_orderLines.Any(x => x.ProductId == productId))
            throw new DomainException("Product already exists in this order.");

        var line = new OrderLine(
            productId,
            quantity,
            unitPrice,
            discountPercentage);

        _orderLines.Add(line);

        RecalculateTotal();
    }

    public void RemoveLine(int productId)
    {
        EnsurePending();

        var line = _orderLines.SingleOrDefault(x => x.ProductId == productId)
                   ?? throw new DomainException("Order line was not found.");

        _orderLines.Remove(line);

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = _orderLines.Sum(x => 
            (x.Quantity * x.UnitPrice) * (1 - x.DiscountPercentage / 100));
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be modified.");
    }

    public void Ship()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be shipped.");

        if (_orderLines.Count == 0)
            throw new DomainException("Cannot ship an empty order.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new DomainException("Delivered orders cannot be cancelled.");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled.");

        Status = OrderStatus.Cancelled;
    }
}
