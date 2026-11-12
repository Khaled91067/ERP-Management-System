namespace ERP.Application.Features.HR.Handlers;

using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions.Caching;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Common.Caching;
using ERP.Application.Common.Models;
using ERP.Application.Features.HR.Dtos;
using ERP.Application.Features.HR.Queries;
using ERP.Domain.HR.Employees;
using MediatR;
using Microsoft.Extensions.Options;

public sealed class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICacheService _cacheService;
    private readonly IOptions<CacheSettings> _cacheSettings;

    public GetEmployeesQueryHandler(
        IEmployeeRepository employeeRepository,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings)
    {
        _employeeRepository = employeeRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var depPart = request.DepartmentId.HasValue ? request.DepartmentId.Value.ToString() : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim() : "none";
        var cacheKey = CacheKeys.HR.EmployeesList(depPart, searchPart, page, pageSize);
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                var options = new QueryOptions<Employee>
                {
                    AsNoTracking = true,
                    OrderBy = q => System.Linq.Queryable.ThenBy(System.Linq.Queryable.OrderBy(q, e => e.FirstName), e => e.LastName)
                };
                options.Includes.Add(e => e.Department);

                if (request.DepartmentId.HasValue)
                {
                    options.Filters.Add(e => e.DepartmentId == request.DepartmentId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    options.Filters.Add(e => e.FirstName.Contains(search) ||
                                             e.LastName.Contains(search) ||
                                             e.Email.Value.Contains(search) ||
                                             e.Position.Contains(search));
                }

                var pagedEmployees = await _employeeRepository.GetPagedAsync(options, page, pageSize, ct);

                return pagedEmployees.Map(employee => new EmployeeDto(
                    employee.Id,
                    employee.FirstName,
                    employee.LastName,
                    employee.Email.Value,
                    employee.Phone,
                    employee.DepartmentId,
                    employee.Department?.Name ?? string.Empty,
                    employee.Position,
                    employee.HireDate,
                    employee.Salary.Amount));
            },
            expiration,
            false,
            cancellationToken);
    }
}
