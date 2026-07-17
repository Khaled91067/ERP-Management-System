using ERP.Domain.Exceptions;

namespace ERP.Domain.Entities;

public class InvoiceLine
{
    public int Id { get; private set; }
    public int InvoiceId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }

    public Invoice? Invoice { get; private set; }

    private InvoiceLine() { }

    internal InvoiceLine(string description, int quantity, decimal unitPrice, decimal taxRate = 0)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Invoice line description is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (taxRate < 0 || taxRate > 100)
            throw new DomainException("Tax rate must be between 0 and 100.");

        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
    }
}
