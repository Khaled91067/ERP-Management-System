using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdWithDepartmentAsync(request.Id, cancellationToken);
        if (employee is null)
            return null;

        return new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Phone,
            employee.DepartmentId,
            employee.Department?.Name ?? string.Empty,
            employee.Position,
            employee.HireDate,
            employee.Salary);
    }
}
