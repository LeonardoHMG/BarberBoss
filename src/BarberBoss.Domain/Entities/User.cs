namespace BarberBoss.Domain.Entities;
public class User
{
    public Guid Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; set; }

    protected User() { }

    public User(
        string name,
        string email,
        string passwordHash,
        string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;

        var now = DateTime.Now;
        CreatedAt = now;
        UpdatedAt = now;
    }
}