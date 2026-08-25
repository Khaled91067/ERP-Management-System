
namespace ERP.Application.Features.HR.Queries;

using ERP.Application.Features.HR.Dtos;

using MediatR;

public sealed record GetEmployeeByIdQuery(
    int Id
) : IRequest<EmployeeDto?>;

