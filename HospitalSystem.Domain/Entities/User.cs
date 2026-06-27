using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Doctor? Doctor { get; private set; }
    public Patient? Patient { get; private set; }
    public Receptionist? Receptionist { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, UserRole role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
