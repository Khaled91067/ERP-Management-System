using MediatR;
using System;

namespace ERP.Application.Features.HR.Commands.Models;

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
