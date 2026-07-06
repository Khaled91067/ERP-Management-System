using System;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
