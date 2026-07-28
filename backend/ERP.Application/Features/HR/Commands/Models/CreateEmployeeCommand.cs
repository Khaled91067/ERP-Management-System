
namespace ERP.Application.Features.HR.Commands.Models;

using System;

using MediatR;

public sealed record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    int DepartmentId,
    string Position,
    DateTime HireDate,
    decimal Salary
) : IRequest<int>;
