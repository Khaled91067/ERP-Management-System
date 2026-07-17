using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Domain.Entities.Orders;
using ERP.Domain.Enums;
using ERP.Domain.Exceptions;

namespace ERP.Domain.Entities;

public class Invoice
{
    private readonly List<InvoiceLine> _invoiceLines = [];

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int CustomerId { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public Order? Order { get; private set; }
    public Customer? Customer { get; private set; }
    public IReadOnlyCollection<InvoiceLine> InvoiceLines => _invoiceLines.AsReadOnly();

    private Invoice() { }

    public Invoice(int orderId, int customerId, DateTime dueDate)
    {
        if (orderId <= 0)
            throw new DomainException("Order ID must be valid.");

        if (customerId <= 0)
            throw new DomainException("Customer ID must be valid.");

        if (dueDate < DateTime.UtcNow.Date)
            throw new DomainException("Due date cannot be in the past.");

        OrderId = orderId;
        CustomerId = customerId;
        InvoiceDate = DateTime.UtcNow;
        DueDate = dueDate;
        Status = InvoiceStatus.Draft;
        TotalAmount = 0;
    }

    public void AddLine(string description, int quantity, decimal unitPrice, decimal taxRate = 0)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Lines can only be added to draft invoices.");

        var line = new InvoiceLine(description, quantity, unitPrice, taxRate);
        _invoiceLines.Add(line);

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = _invoiceLines.Sum(x => 
            (x.Quantity * x.UnitPrice) * (1 + x.TaxRate / 100));
    }

    public void Send()
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Only draft invoices can be sent.");

        if (_invoiceLines.Count == 0)
            throw new DomainException("Cannot send an empty invoice.");

        Status = InvoiceStatus.Sent;
    }

    public void Pay()
    {
        if (Status != InvoiceStatus.Sent)
            throw new DomainException("Only sent invoices can be marked as paid.");

        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Paid invoices cannot be cancelled.");

        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Invoice is already cancelled.");

        Status = InvoiceStatus.Cancelled;
    }
}
