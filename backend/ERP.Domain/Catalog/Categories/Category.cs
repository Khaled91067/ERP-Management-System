
namespace ERP.Domain.Catalog.Categories;

using ERP.Domain.Catalog.Products;
using ERP.Domain.Shared.Common;

public class Category : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
