
namespace ERP.Application.Features.HR.Commands.Models;

using MediatR;

public sealed record DeleteEmployeeCommand(
    int Id
) : IRequest<bool>;
