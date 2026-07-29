
namespace ERP.Application.Features.HR.Commands;

using MediatR;

public sealed record DeleteEmployeeCommand(
    int Id
) : IRequest<bool>;
