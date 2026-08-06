namespace ERP.Domain.Catalog.Products;

using System.Collections.Generic;

using ERP.Domain.Catalog.Categories;
using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class Product : SoftDeletableEntity
{
    private readonly List<OrderLine> _orderLineList = [];
    private readonly List<PurchaseOrderLine> _purchaseOrderLineList = [];

    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public int CategoryId { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money CostPrice { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public int ReorderLevel { get; private set; }
    public Category? Category { get; private set; }

    public IReadOnlyCollection<OrderLine> OrderLines => _orderLineList.AsReadOnly();
    public IReadOnlyCollection<PurchaseOrderLine> PurchaseOrderLines => _purchaseOrderLineList.AsReadOnly();

    private Product() { }

    public Product(
        string name,
        string sku,
        int categoryId,
        decimal unitPrice,
        decimal costPrice,
        int reorderLevel)
    {
        ValidateAndAssignDetails(name, sku, categoryId, unitPrice, costPrice, reorderLevel);
    }

    public void UpdateDetails(
        string name,
        string sku,
        int categoryId,
        decimal unitPrice,
        decimal costPrice,
        int reorderLevel)
    {
        ValidateAndAssignDetails(name, sku, categoryId, unitPrice, costPrice, reorderLevel);
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleValidationException(
                "Stock increase quantity must be greater than zero.");

        StockQuantity += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleValidationException(
                "Stock decrease quantity must be greater than zero.");

        if (StockQuantity < quantity)
            throw new BusinessRuleValidationException(
                $"Insufficient stock for product {Name}. Available: {StockQuantity}, Requested: {quantity}");

        StockQuantity -= quantity;
    }

    private void ValidateAndAssignDetails(
        string name,
        string sku,
        int categoryId,
        decimal unitPrice,
        decimal costPrice,
        int reorderLevel)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Product name is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new BusinessRuleValidationException("Product SKU is required.");

        if (categoryId <= 0)
            throw new BusinessRuleValidationException("Category id must be valid.");

        if (reorderLevel < 0)
            throw new BusinessRuleValidationException("Prices and reorder level cannot be negative.");

        Name = name.Trim();
        Sku = sku.Trim();
        CategoryId = categoryId;
        UnitPrice = new Money(unitPrice);
        CostPrice = new Money(costPrice);
        ReorderLevel = reorderLevel;
    }
}
