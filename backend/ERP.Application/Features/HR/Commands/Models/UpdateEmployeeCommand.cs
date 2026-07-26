using MediatR;
using System;

namespace ERP.Application.Features.HR.Commands.Models;

public sealed record UpdateEmployeeCommand(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    int DepartmentId,
    string Position,
    DateTime HireDate,
    decimal Salary
) : IRequest<bool>;
