namespace ERP.Domain.HR.Employees;

using System;
using ERP.Domain.HR.Departments;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;
using ERP.Domain.Shared.Exceptions;

public class Employee : BaseEntity, ISoftDeletable
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public int DepartmentId { get; private set; }
    public string Position { get; private set; } = string.Empty;
    public DateTime HireDate { get; private set; }
    public decimal Salary { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public Department? Department { get; private set; }

    private Employee() { }

    public Employee(
        string firstName,
        string lastName,
        string email,
        string phone,
        int departmentId,
        string position,
        DateTime hireDate,
        decimal salary)
    {
        ValidateAndAssignDetails(firstName, lastName, email, phone, departmentId, position, hireDate, salary);
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        string email,
        string phone,
        int departmentId,
        string position,
        DateTime hireDate,
        decimal salary)
    {
        ValidateAndAssignDetails(firstName, lastName, email, phone, departmentId, position, hireDate, salary);
    }

    private void ValidateAndAssignDetails(
        string firstName,
        string lastName,
        string email,
        string phone,
        int departmentId,
        string position,
        DateTime hireDate,
        decimal salary)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleValidationException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleValidationException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessRuleValidationException("Email is required.");

        if (departmentId <= 0)
            throw new BusinessRuleValidationException("Department ID must be valid.");

        if (string.IsNullOrWhiteSpace(position))
            throw new BusinessRuleValidationException("Position is required.");

        if (salary < 0)
            throw new BusinessRuleValidationException("Salary cannot be negative.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        Phone = phone?.Trim() ?? string.Empty;
        DepartmentId = departmentId;
        Position = position.Trim();
        HireDate = hireDate;
        Salary = salary;
    }
}
