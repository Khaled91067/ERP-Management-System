namespace ERP.Domain.Sales.Customers;

using System.Collections.Generic;
using ERP.Domain.Sales.Invoices;
using ERP.Domain.Sales.Orders;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Exceptions;
using ERP.Domain.Shared.ValueObjects;

public class Customer : SoftDeletableEntity
{
    private readonly List<Order> _orders = [];
    private readonly List<Invoice> _invoices = [];

    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;

    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    private Customer() { }

    public Customer(
        string name,
        string email,
        string phone = "",
        string address = "",
        string city = "",
        string country = "",
        string taxId = "")
    {
        ValidateAndAssignDetails(name, email, phone, address, city, country, taxId);
    }

    public void UpdateDetails(
        string name,
        string email,
        string phone,
        string address,
        string city,
        string country,
        string taxId)
    {
        ValidateAndAssignDetails(name, email, phone, address, city, country, taxId);
    }

    private void ValidateAndAssignDetails(
        string name,
        string email,
        string phone,
        string address,
        string city,
        string country,
        string taxId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Customer name is required.");

        Name = name.Trim();
        Email = new Email(email);
        Phone = phone?.Trim() ?? string.Empty;
        Address = address?.Trim() ?? string.Empty;
        City = city?.Trim() ?? string.Empty;
        Country = country?.Trim() ?? string.Empty;
        TaxId = taxId?.Trim() ?? string.Empty;
    }
}
