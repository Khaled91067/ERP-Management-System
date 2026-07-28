
namespace ERP.Application.Features.HR.Handlers;

using System.Collections.Generic;

using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Models;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries.Models;
using ERP.Domain.HR.Departments;

using MediatR;

public sealed class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResult<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<PagedResult<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Department>
        {
            Includes = new List<System.Linq.Expressions.Expression<System.Func<Department, object>>>
            {
                d => d.Employees
            }
        };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            options.Filter = d => d.Name.ToLower().Contains(search);
        }

        var pagedDepartments = await _departmentRepository.GetPagedAsync(options, request.Page, request.PageSize);

        return pagedDepartments.Map(d => new DepartmentDto(
            d.Id,
            d.Name,
            d.Employees.Count));
    }
}
