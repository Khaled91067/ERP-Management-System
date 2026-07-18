using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetEmployeesWithDepartmentAsync(request.DepartmentId, cancellationToken);

        // Apply Search Term filter in-memory if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            employees = employees.Where(x => 
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search) ||
                x.Position.ToLower().Contains(search)
            ).ToList();
        }

        return employees.Select(employee => new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Phone,
            employee.DepartmentId,
            employee.Department?.Name ?? string.Empty,
            employee.Position,
            employee.HireDate,
            employee.Salary)).ToList();
    }
}
