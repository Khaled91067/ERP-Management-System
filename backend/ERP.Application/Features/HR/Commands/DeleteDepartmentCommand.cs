
namespace ERP.Application.Features.HR.Commands;

using MediatR;

public sealed record DeleteDepartmentCommand(
    int Id
) : IRequest<bool>;
