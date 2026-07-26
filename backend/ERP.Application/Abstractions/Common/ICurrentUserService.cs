namespace ERP.Application.Abstractions.Common;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
}
