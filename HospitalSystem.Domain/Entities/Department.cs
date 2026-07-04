namespace HospitalSystem.Domain.Entities;

public class Department
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<Doctor> Doctors { get; private set; } = new List<Doctor>();

    private Department() { }

    public static Department Create(string name, string? description = null)
    {
        return new Department
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
