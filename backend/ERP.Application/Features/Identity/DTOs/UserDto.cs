namespace ERP.Application.Features.Identity.DTOs;

public sealed record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    int RoleId,
    string RoleName);
