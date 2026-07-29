
namespace ERP.Application.Features.HR.Commands;

using MediatR;

public sealed record CreateDepartmentCommand(
    string Name
) : IRequest<int>;
