using System;

namespace ERP.Application.Features.HR.Dtos;

public sealed record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    int DepartmentId,
    string DepartmentName,
    string Position,
    DateTime HireDate,
    decimal Salary);
