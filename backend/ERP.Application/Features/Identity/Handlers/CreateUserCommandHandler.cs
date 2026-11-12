namespace ERP.Application.Features.Identity.Handlers;

using System.Threading;
using System.Threading.Tasks;
using ERP.Application.Abstractions;
using ERP.Application.Abstractions.Authentication;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Identity.Commands;
using ERP.Domain.Identity.Users;
using ERP.Domain.Shared.Exceptions;
using MediatR;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.EmailExistsAsync(request.Email.Trim(), cancellationToken))
            throw new ConflictException("Email is already registered.");
        if (await roleRepository.GetByIdAsync(request.RoleId, cancellationToken) is null)
            throw new NotFoundException("Role", request.RoleId);

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHasher.Hash(request.Password),
            request.RoleId);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}
