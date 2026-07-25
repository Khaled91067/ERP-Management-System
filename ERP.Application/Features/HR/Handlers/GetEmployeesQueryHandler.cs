using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries.Models;
using ERP.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace ERP.Application.Features.HR.Handlers;

public sealed class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Employee>();
        options.Includes.Add(e => e.Department);

        if (request.DepartmentId.HasValue)
        {
            options.Filter = e => e.DepartmentId == request.DepartmentId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            var existingFilter = options.Filter;
            options.Filter = e => (existingFilter == null || existingFilter.Compile()(e)) &&
                                  (e.FirstName.ToLower().Contains(search) ||
                                   e.LastName.ToLower().Contains(search) ||
                                   e.Email.ToLower().Contains(search) ||
                                   e.Position.ToLower().Contains(search));
        }

        var pagedEmployees = await _employeeRepository.GetPagedAsync(options, request.Page, request.PageSize);

        return pagedEmployees.Map(employee => new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Phone,
            employee.DepartmentId,
            employee.Department?.Name ?? string.Empty,
            employee.Position,
            employee.HireDate,
            employee.Salary));
    }
}
