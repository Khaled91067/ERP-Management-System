
namespace ERP.Domain.HR.Departments;

using ERP.Domain.HR.Employees;
using ERP.Domain.Shared.Common;

public class Department : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
