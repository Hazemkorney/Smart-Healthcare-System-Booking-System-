using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateOnly AppointmentDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CreatedByReceptionistId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;
    public User? CreatedByReceptionist { get; private set; }
    public Consultation? Consultation { get; private set; }

    private Appointment() { }

    public static Appointment Create(
        Guid patientId,
        Guid doctorId,
        DateOnly appointmentDate,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? createdByReceptionistId = null,
        string? notes = null)
    {
        var now = DateTime.UtcNow;
        return new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = appointmentDate,
            StartTime = startTime,
            EndTime = endTime,
            Status = AppointmentStatus.Confirmed,
            Notes = notes,
            CreatedByReceptionistId = createdByReceptionistId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Reschedule(DateOnly newDate, TimeSpan newStartTime, TimeSpan newEndTime)
    {
        AppointmentDate = newDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CheckIn()
    {
        Status = AppointmentStatus.CheckedIn;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartConsultation()
    {
        Status = AppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = AppointmentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkNoShow()
    {
        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
