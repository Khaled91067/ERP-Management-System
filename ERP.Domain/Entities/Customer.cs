using ERP.Domain.Entities.Orders;

namespace ERP.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
