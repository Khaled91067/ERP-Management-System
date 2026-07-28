namespace ERP.Domain.HR.Departments;

using System.Collections.Generic;
using ERP.Domain.HR.Employees;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;
using ERP.Domain.Shared.Exceptions;

public class Department : BaseEntity, ISoftDeletable
{
    private readonly List<Employee> _employees = [];

    public string Name { get; private set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Department() { }

    public Department(string name)
    {
        UpdateName(name);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Department name is required.");

        Name = name.Trim();
    }
}
