using ERP.Application.Abstractions.Authentication;
using ERP.Application.Abstractions.Repositories;
using ERP.Application.Features.Authentication.Commands.Models;
using ERP.Application.Features.Authentication.DTOs;
using MediatR;

namespace ERP.Application.Features.Authentication.Commands.Handlers;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var passwordIsValid = _passwordHasher.Verify(request.Password,user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var accessToken = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResponse(accessToken);
    }
}