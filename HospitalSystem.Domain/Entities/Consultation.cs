namespace HospitalSystem.Domain.Entities;

public class Consultation
{
    public Guid Id { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? Notes { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public Appointment Appointment { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;
    public Patient Patient { get; private set; } = null!;
    public ICollection<Prescription> Prescriptions { get; private set; } = new List<Prescription>();

    private Consultation() { }

    public static Consultation Create(Guid appointmentId, Guid doctorId, Guid patientId)
    {
        return new Consultation
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            DoctorId = doctorId,
            PatientId = patientId,
            StartedAt = DateTime.UtcNow
        };
    }

    public void AddDiagnosis(string diagnosis, string? notes = null)
    {
        Diagnosis = diagnosis;
        Notes = notes;
    }

    public void Complete()
    {
        CompletedAt = DateTime.UtcNow;
    }
}
