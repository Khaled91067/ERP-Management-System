namespace ERP.Domain.Purchasing.PurchaseOrders;

using System;
using System.Collections.Generic;
using System.Linq;

using ERP.Domain.Purchasing.Suppliers;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class PurchaseOrder : AggregateRoot
{
    private readonly List<PurchaseOrderLine> _purchaseOrderLines = [];

    public int SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime ExpectedDelivery { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public Supplier? Supplier { get; private set; }

    public IReadOnlyCollection<PurchaseOrderLine> PurchaseOrderLines =>
        _purchaseOrderLines.AsReadOnly();

    private PurchaseOrder() { }

    public PurchaseOrder(
        int supplierId,
        DateTime expectedDelivery)
    {
        var orderDate = DateTime.UtcNow;

        if (supplierId <= 0)
            throw new BusinessRuleValidationException(
                "Supplier id must be valid.");

        if (expectedDelivery < orderDate)
            throw new BusinessRuleValidationException(
                "Expected delivery cannot be before order date.");

        SupplierId = supplierId;
        OrderDate = orderDate;
        ExpectedDelivery = expectedDelivery;
        Status = PurchaseOrderStatus.Draft;
        TotalAmount = new Money(0);
    }

    public void AddLine(
        int productId,
        int quantity,
        decimal unitCost)
    {
        EnsureDraft();

        if (_purchaseOrderLines.Any(x => x.ProductId == productId))
            throw new ConflictException(
                "Product already exists in this purchase order.");

        var line = new PurchaseOrderLine(
            productId,
            quantity,
            unitCost);

        _purchaseOrderLines.Add(line);

        RecalculateTotal();
    }

    public void RemoveLine(int productId)
    {
        EnsureDraft();

        var line = GetLine(productId);

        _purchaseOrderLines.Remove(line);

        RecalculateTotal();
    }

    public void ChangeLineQuantity(
        int productId,
        int quantity)
    {
        EnsureDraft();

        var line = GetLine(productId);

        line.ChangeQuantity(quantity);

        RecalculateTotal();
    }

    public void ChangeLineUnitCost(
        int productId,
        decimal unitCost)
    {
        EnsureDraft();

        var line = GetLine(productId);

        line.ChangeUnitCost(unitCost);

        RecalculateTotal();
    }

    public void EnsureCanBeDeleted()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new BusinessRuleValidationException(
                "Only draft purchase orders can be deleted.");
    }

    private PurchaseOrderLine GetLine(int productId)
    {
        return _purchaseOrderLines.SingleOrDefault(
                   x => x.ProductId == productId)
               ?? throw new NotFoundException(
                   "Purchase line was not found.");
    }

    private void RecalculateTotal()
    {
        TotalAmount = new Money(_purchaseOrderLines.Sum(
            x => x.Quantity * x.UnitCost.Amount));
    }

    private void EnsureDraft()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new BusinessRuleValidationException(
                "Only draft purchase orders can be modified.");
    }

    public void Submit()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new BusinessRuleValidationException(
                "Only draft purchase orders can be submitted.");

        if (_purchaseOrderLines.Count == 0)
            throw new BusinessRuleValidationException(
                "Cannot submit an empty purchase order.");

        Status = PurchaseOrderStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new BusinessRuleValidationException(
                "Only draft purchase orders can be approved.");

        if (_purchaseOrderLines.Count == 0)
            throw new BusinessRuleValidationException(
                "Cannot approve an empty purchase order.");

        Status = PurchaseOrderStatus.Approved;
    }

    public void Receive()
    {
        if (Status != PurchaseOrderStatus.Approved)
            throw new BusinessRuleValidationException(
                "Only approved purchase orders can be received.");

        Status = PurchaseOrderStatus.Received;

        AddDomainEvent(new PurchaseOrderReceivedDomainEvent(Id));
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Received)
            throw new BusinessRuleValidationException(
                "Received purchase orders cannot be cancelled.");

        if (Status == PurchaseOrderStatus.Cancelled)
            throw new BusinessRuleValidationException(
                "Purchase order is already cancelled.");

        Status = PurchaseOrderStatus.Cancelled;
    }
}