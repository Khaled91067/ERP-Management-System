using System;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? PaidAt { get; set; }

    public Order? Order { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
