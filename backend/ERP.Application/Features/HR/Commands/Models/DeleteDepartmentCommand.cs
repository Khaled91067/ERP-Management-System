
namespace ERP.Application.Features.HR.Commands.Models;

using MediatR;

public sealed record DeleteDepartmentCommand(
    int Id
) : IRequest<bool>;
