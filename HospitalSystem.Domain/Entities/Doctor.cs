namespace HospitalSystem.Domain.Entities;

public class Doctor
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Specialization { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }

    public User User { get; private set; } = null!;
    public Department Department { get; private set; } = null!;
    public ICollection<DoctorSchedule> Schedules { get; private set; } = new List<DoctorSchedule>();
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    public ICollection<Consultation> Consultations { get; private set; } = new List<Consultation>();

    private Doctor() { }

    public static Doctor Create(Guid userId, Guid departmentId, string fullName, string specialization, string? phone = null)
    {
        return new Doctor
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DepartmentId = departmentId,
            FullName = fullName,
            Specialization = specialization,
            Phone = phone,
            IsActive = true
        };
    }

    public void Update(string fullName, string specialization, string? phone)
    {
        FullName = fullName;
        Specialization = specialization;
        Phone = phone;
    }

    public void AssignToDepartment(Guid departmentId) => DepartmentId = departmentId;

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
