namespace HospitalSystem.Domain.Entities;

public class Receptionist
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }

    public User User { get; private set; } = null!;

    private Receptionist() { }

    public static Receptionist Create(Guid userId, string fullName, string? phone = null)
    {
        return new Receptionist
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = fullName,
            Phone = phone,
            IsActive = true
        };
    }

    public void Update(string fullName, string? phone)
    {
        FullName = fullName;
        Phone = phone;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
