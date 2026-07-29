namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Common.Models;
using global::ERP.Application.Features.Identity.Commands;
using global::ERP.Application.Features.Identity.DTOs;
using global::ERP.Application.Features.Identity.Queries;
using global::ERP.Domain.Identity.Users;

using MediatR;

public sealed class DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null) return false;

        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

