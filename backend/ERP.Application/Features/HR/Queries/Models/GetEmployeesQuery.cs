
namespace ERP.Application.Features.HR.Queries.Models;

using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.HR.Dtos;

using MediatR;

public sealed record GetEmployeesQuery(
    int? DepartmentId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<EmployeeDto>>;

