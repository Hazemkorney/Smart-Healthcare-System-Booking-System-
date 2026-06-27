using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? NationalId { get; private set; }
    public string? BloodType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User? User { get; private set; }
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    public ICollection<Consultation> Consultations { get; private set; } = new List<Consultation>();

    private Patient() { }

    public static Patient Create(
        string fullName,
        DateTime dateOfBirth,
        Gender gender,
        string phone,
        string? email = null,
        string? address = null,
        string? nationalId = null,
        string? bloodType = null,
        Guid? userId = null)
    {
        return new Patient
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = fullName,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            Phone = phone,
            Email = email,
            Address = address,
            NationalId = nationalId,
            BloodType = bloodType,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string fullName,
        DateTime dateOfBirth,
        Gender gender,
        string phone,
        string? email,
        string? address,
        string? nationalId,
        string? bloodType)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Phone = phone;
        Email = email;
        Address = address;
        NationalId = nationalId;
        BloodType = bloodType;
    }

    public void LinkUser(Guid userId) => UserId = userId;
}
