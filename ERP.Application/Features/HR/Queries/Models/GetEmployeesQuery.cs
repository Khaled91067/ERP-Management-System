using ERP.Application.Features.HR.Dtos;
using MediatR;
using System.Collections.Generic;

namespace ERP.Application.Features.HR.Queries.Models;

public sealed record GetEmployeesQuery(
    int? DepartmentId = null,
    string? SearchTerm = null
) : IRequest<IEnumerable<EmployeeDto>>;
