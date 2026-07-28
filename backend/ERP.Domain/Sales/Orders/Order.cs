namespace ERP.Domain.Sales.Orders;

using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Domain.Sales.Customers;
using ERP.Domain.Sales.Invoices;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Order : SoftDeletableEntity
{
    private readonly List<OrderLine> _orderLines = [];
    private readonly List<Invoice> _invoices = [];

    public int CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string ShippingAddress { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public Customer? Customer { get; private set; }

    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    private Order() { }

    public Order(
        int customerId,
        PaymentMethod paymentMethod,
        string shippingAddress)
    {
        if (customerId <= 0)
            throw new BusinessRuleValidationException("Customer id must be valid.");

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new BusinessRuleValidationException("Shipping address is required.");

        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        ShippingAddress = shippingAddress.Trim();

        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public void UpdateShippingAddress(string shippingAddress)
    {
        EnsurePending();

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new BusinessRuleValidationException("Shipping address is required.");

        ShippingAddress = shippingAddress.Trim();
    }

    public void AddLine(
        int productId,
        int quantity,
        decimal unitPrice,
        decimal discountPercentage = 0)
    {
        EnsurePending();

        if (_orderLines.Any(x => x.ProductId == productId))
            throw new ConflictException("Product already exists in this order.");

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
                   ?? throw new NotFoundException("Order line was not found.");

        _orderLines.Remove(line);

        RecalculateTotal();
    }

    public void EnsureCanBeDeleted()
    {
        if (Status != OrderStatus.Pending)
            throw new BusinessRuleValidationException("Only pending orders can be deleted.");
    }

    private void RecalculateTotal()
    {
        TotalAmount = _orderLines.Sum(x =>
            (x.Quantity * x.UnitPrice) * (1 - x.DiscountPercentage / 100));
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
            throw new BusinessRuleValidationException("Only pending orders can be modified.");
    }

    public void Ship()
    {
        if (Status != OrderStatus.Pending)
            throw new BusinessRuleValidationException("Only pending orders can be shipped.");

        if (_orderLines.Count == 0)
            throw new BusinessRuleValidationException("Cannot ship an empty order.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new BusinessRuleValidationException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new BusinessRuleValidationException("Delivered orders cannot be cancelled.");

        if (Status == OrderStatus.Cancelled)
            throw new BusinessRuleValidationException("Order is already cancelled.");

        Status = OrderStatus.Cancelled;
    }
}
