namespace ERP.Application.Features.HR.Handlers;

using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.HR.Dtos;
using global::ERP.Application.Features.HR.Queries;
using global::ERP.Domain.HR.Employees;

using MediatR;

public sealed class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly global::ERP.Application.Abstractions.Caching.ICacheService _cacheService;
    private readonly global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> _cacheSettings;

    public GetEmployeesQueryHandler(
        IEmployeeRepository employeeRepository,
        global::ERP.Application.Abstractions.Caching.ICacheService cacheService,
        global::Microsoft.Extensions.Options.IOptions<global::ERP.Application.Common.Caching.CacheSettings> cacheSettings)
    {
        _employeeRepository = employeeRepository;
        _cacheService = cacheService;
        _cacheSettings = cacheSettings;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var depPart = request.DepartmentId.HasValue ? request.DepartmentId.Value.ToString() : "all";
        var searchPart = !string.IsNullOrWhiteSpace(request.Search) ? request.Search.Trim().ToLower() : "none";
        var cacheKey = $"HR:Employees:Dep={depPart}:Search={searchPart}:Page={request.Page}:Size={request.PageSize}";
        var expiration = TimeSpan.FromMinutes(_cacheSettings.Value.PaginatedListExpirationMinutes);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
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
                                           e.Email.Value.ToLower().Contains(search) ||
                                           e.Position.ToLower().Contains(search));
                }

                var pagedEmployees = await _employeeRepository.GetPagedAsync(options, request.Page, request.PageSize);

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


