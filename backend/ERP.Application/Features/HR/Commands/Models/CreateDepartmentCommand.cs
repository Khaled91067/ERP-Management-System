
namespace ERP.Application.Features.HR.Commands.Models;

using MediatR;

public sealed record CreateDepartmentCommand(
    string Name
) : IRequest<int>;
