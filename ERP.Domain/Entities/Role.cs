namespace ERP.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Storing as JSON string
    public string Permissions { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}
