namespace ERP.Application.Features.Identity.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;
using ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;
        if (await userRepository.EmailExistsExceptAsync(request.Email.Trim(), user.Id, cancellationToken))
            throw new ConflictException("Email is already registered.");

        user.UpdateProfile(request.FirstName, request.LastName, request.Email);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
