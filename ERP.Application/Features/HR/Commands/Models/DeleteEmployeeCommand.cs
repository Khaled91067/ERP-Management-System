using MediatR;

namespace ERP.Application.Features.HR.Commands.Models;

public sealed record DeleteEmployeeCommand(
    int Id
) : IRequest<bool>;
