namespace ERP.Domain.Sales.Invoices;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class InvoiceLine : BaseEntity
{
    public int InvoiceId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public decimal TaxRate { get; private set; }

    public Invoice? Invoice { get; private set; }

    private InvoiceLine() { }

    internal InvoiceLine(string description, int quantity, decimal unitPrice, decimal taxRate = 0)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessRuleValidationException("Invoice line description is required.");

        if (quantity <= 0)
            throw new BusinessRuleValidationException("Quantity must be greater than zero.");

        if (taxRate < 0 || taxRate > 100)
            throw new BusinessRuleValidationException("Tax rate must be between 0 and 100.");

        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = new Money(unitPrice);
        TaxRate = taxRate;
    }
}
