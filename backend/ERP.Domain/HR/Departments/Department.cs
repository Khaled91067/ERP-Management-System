
namespace ERP.Domain.HR.Departments;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using ERP.Domain.HR.Employees;
using ERP.Domain.Shared.Base;

public class Department : BaseEntity, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
