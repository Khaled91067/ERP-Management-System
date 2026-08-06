namespace ERP.Application.Features.Identity.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Identity.Commands;

using MediatR;

public sealed class UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;
        if (await userRepository.EmailExistsExceptAsync(request.Email.Trim(), user.Id, cancellationToken))
            throw new InvalidOperationException("Email is already registered.");

        user.UpdateProfile(request.FirstName, request.LastName, request.Email);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

