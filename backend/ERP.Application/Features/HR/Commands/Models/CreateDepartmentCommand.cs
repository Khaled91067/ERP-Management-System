using MediatR;

namespace ERP.Application.Features.HR.Commands.Models;

public sealed record CreateDepartmentCommand(
    string Name
) : IRequest<int>;
