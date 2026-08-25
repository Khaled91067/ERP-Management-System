
namespace ERP.Application.Features.HR.Queries;

using ERP.Application.Common.Models;
using ERP.Application.Features.HR.Dtos;

using MediatR;

public sealed record GetEmployeesQuery(
    int? DepartmentId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<EmployeeDto>>;

