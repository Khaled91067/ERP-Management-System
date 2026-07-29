namespace ERP.Application.Features.Authentication.Commands.Handlers;

using global::ERP.Application.Abstractions;
using global::ERP.Application.Abstractions.Authentication;
using global::ERP.Application.Abstractions.Repositories;
using global::ERP.Application.Features.Authentication.Commands.Models;
using global::ERP.Domain.Identity.Users;

using MediatR;

public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
            throw new InvalidOperationException("Email is already registered.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash,
            1); // Default role ID

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
