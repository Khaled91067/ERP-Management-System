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
        EnsureOrderCanBeModified();

        var line = new OrderLine(
            productId,
            quantity,
            unitPrice,
            discountPercentage);

        _orderLines.Add(line);

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = _orderLines.Sum(
            line => line.CalculateTotal());
    }

    public void ChangeLineQuantity(
    int productId,
    int quantity)
    {
        EnsureOrderCanBeModified();

        var line = _orderLines
            .FirstOrDefault(line =>
                line.ProductId == productId);

        if (line is null)
            throw new DomainException(
                "Order line not found.");

        line.ChangeQuantity(quantity);

        RecalculateTotal();
    }

    public void RemoveLine(int productId)
    {
        EnsureOrderCanBeModified();

        var line = _orderLines
            .FirstOrDefault(line =>
                line.ProductId == productId);

        if (line is null)
            throw new DomainException(
                "Order line not found.");

        _orderLines.Remove(line);

        RecalculateTotal();
    }

    private void EnsureOrderCanBeModified()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(
                "Only pending orders can be modified.");
    }
}


