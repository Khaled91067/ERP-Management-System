namespace ERP.Domain.Purchasing.Suppliers;

using System.Collections.Generic;
using ERP.Domain.Purchasing.PurchaseOrders;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class Supplier : SoftDeletableEntity
{
    private readonly List<PurchaseOrder> _purchaseOrders = [];

    public string CompanyName { get; private set; } = string.Empty;
    public string ContactName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string Phone { get; private set; } = string.Empty;
    public string PaymentTerms { get; private set; } = string.Empty;

    public IReadOnlyCollection<PurchaseOrder> PurchaseOrders => _purchaseOrders.AsReadOnly();

    private Supplier() { }

    public Supplier(
        string companyName,
        string email,
        string contactName = "",
        string phone = "",
        string paymentTerms = "")
    {
        ValidateAndAssignDetails(companyName, email, contactName, phone, paymentTerms);
    }

    public void UpdateDetails(
        string companyName,
        string email,
        string contactName,
        string phone,
        string paymentTerms)
    {
        ValidateAndAssignDetails(companyName, email, contactName, phone, paymentTerms);
    }

    private void ValidateAndAssignDetails(
        string companyName,
        string email,
        string contactName,
        string phone,
        string paymentTerms)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new BusinessRuleValidationException("Supplier company name is required.");

        CompanyName = companyName.Trim();
        Email = new Email(email);
        ContactName = contactName?.Trim() ?? string.Empty;
        Phone = phone?.Trim() ?? string.Empty;
        PaymentTerms = paymentTerms?.Trim() ?? string.Empty;
    }
}
