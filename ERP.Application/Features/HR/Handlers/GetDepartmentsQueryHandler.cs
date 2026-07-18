using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries.Models;
using ERP.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Application.Features.HR.Handlers;

public sealed class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, IEnumerable<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var options = new QueryOptions<Department>
        {
            Includes = new List<System.Linq.Expressions.Expression<System.Func<Department, object>>>
            {
                d => d.Employees
            }
        };

        var departments = await _departmentRepository.GetAllAsync(options);

        return departments.Select(d => new DepartmentDto(
            d.Id,
            d.Name,
            d.Employees.Count));
    }
}
