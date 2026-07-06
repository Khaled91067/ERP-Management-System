using System;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDelivery { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }

    public Supplier? Supplier { get; set; }
    public ICollection<PurchaseLine> PurchaseLines { get; set; } = new List<PurchaseLine>();
}
