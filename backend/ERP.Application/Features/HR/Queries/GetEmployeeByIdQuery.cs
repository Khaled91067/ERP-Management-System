
namespace ERP.Application.Features.HR.Queries;

using global::ERP.Application.Features.HR.Dtos;

using MediatR;

public sealed record GetEmployeeByIdQuery(
    int Id
) : IRequest<EmployeeDto?>;

