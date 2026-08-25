namespace ERP.Application.Features.Identity.Handlers;

using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed class DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;

        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

