using ERP.Domain.Common;
using ERP.Domain.Enums;
using ERP.Domain.Events;
using ERP.Domain.Exceptions;
using System;

namespace ERP.Domain.Entities;

public class PurchaseOrder : AggregateRoot, ISoftDeletable, IAuditable
{
    private readonly List<PurchaseLine> _purchaseLines = [];

    public int Id { get; private set; }
    public int SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime ExpectedDelivery { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public Supplier? Supplier { get; private set; }

    public IReadOnlyCollection<PurchaseLine> PurchaseLines =>
        _purchaseLines.AsReadOnly();

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
        TotalAmount = 0;
    }

    public void AddLine(
        int productId,
        int quantity,
        decimal unitCost)
    {
        EnsureDraft();

        if (_purchaseLines.Any(x => x.ProductId == productId))
            throw new ConflictException(
                "Product already exists in this purchase order.");

        var line = new PurchaseLine(
            productId,
            quantity,
            unitCost);

        _purchaseLines.Add(line);

        RecalculateTotal();
    }

    public void RemoveLine(int productId)
    {
        EnsureDraft();

        var line = GetLine(productId);

        _purchaseLines.Remove(line);

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

    private PurchaseLine GetLine(int productId)
    {
        return _purchaseLines.SingleOrDefault(
                   x => x.ProductId == productId)
               ?? throw new NotFoundException(
                   "Purchase line was not found.");
    }

    private void RecalculateTotal()
    {
        TotalAmount = _purchaseLines.Sum(
            x => x.Quantity * x.UnitCost);
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

        if (_purchaseLines.Count == 0)
            throw new BusinessRuleValidationException(
                "Cannot submit an empty purchase order.");

        Status = PurchaseOrderStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new BusinessRuleValidationException(
                "Only draft purchase orders can be approved.");

        if (_purchaseLines.Count == 0)
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
       
        AddDomainEvent( new PurchaseOrderReceivedDomainEvent(Id));
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