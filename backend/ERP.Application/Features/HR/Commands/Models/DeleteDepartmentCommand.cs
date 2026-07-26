using MediatR;

namespace ERP.Application.Features.HR.Commands.Models;

public sealed record DeleteDepartmentCommand(
    int Id
) : IRequest<bool>;
