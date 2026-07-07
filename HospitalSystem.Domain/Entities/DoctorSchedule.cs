namespace HospitalSystem.Domain.Entities;

public class DoctorSchedule
{
    public Guid Id { get; private set; }
    public Guid DoctorId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int AppointmentDurationMinutes { get; private set; }
    public bool IsActive { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    private DoctorSchedule() { }

    public static DoctorSchedule Create(
        Guid doctorId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int appointmentDurationMinutes)
    {
        return new DoctorSchedule
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            AppointmentDurationMinutes = appointmentDurationMinutes,
            IsActive = true
        };
    }

    public void Update(TimeSpan startTime, TimeSpan endTime, int appointmentDurationMinutes)
    {
        StartTime = startTime;
        EndTime = endTime;
        AppointmentDurationMinutes = appointmentDurationMinutes;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
