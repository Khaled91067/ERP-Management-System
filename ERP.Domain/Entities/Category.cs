using ERP.Domain.Common;

namespace ERP.Domain.Entities;

public class Category : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
