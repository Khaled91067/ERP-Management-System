namespace ERP.Domain.Identity.Roles;
using ERP.Domain.Shared.Base;
using ERP.Domain.Shared.Abstractions;

using ERP.Domain.Identity.Users;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Storing as JSON string
    public string Permissions { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}
