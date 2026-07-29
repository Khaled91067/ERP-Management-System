
namespace ERP.Application.Features.Authentication.Commands
{
    using MediatR;

    public sealed record LogoutCommand(string RefreshToken) : IRequest;
}
