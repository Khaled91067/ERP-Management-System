using ERP.Domain.Common;

namespace ERP.Domain.Entities;

public class Department : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
