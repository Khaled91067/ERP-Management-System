
namespace ERP.Domain.Sales.Invoices;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;

using ERP.Domain.Sales.Customers;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;

public class Invoice : SoftDeletableEntity
{
    private readonly List<InvoiceLine> _invoiceLines = [];
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
            throw new BusinessRuleValidationException("Order ID must be valid.");

        if (customerId <= 0)
            throw new BusinessRuleValidationException("Customer ID must be valid.");

        if (dueDate < DateTime.UtcNow.Date)
            throw new BusinessRuleValidationException("Due date cannot be in the past.");

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
            throw new BusinessRuleValidationException("Lines can only be added to draft invoices.");

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
            throw new BusinessRuleValidationException("Only draft invoices can be sent.");

        if (_invoiceLines.Count == 0)
            throw new BusinessRuleValidationException("Cannot send an empty invoice.");

        Status = InvoiceStatus.Sent;
    }

    public void Pay()
    {
        if (Status != InvoiceStatus.Sent)
            throw new BusinessRuleValidationException("Only sent invoices can be marked as paid.");

        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new BusinessRuleValidationException("Paid invoices cannot be cancelled.");

        if (Status == InvoiceStatus.Cancelled)
            throw new BusinessRuleValidationException("Invoice is already cancelled.");

        Status = InvoiceStatus.Cancelled;
    }
}
