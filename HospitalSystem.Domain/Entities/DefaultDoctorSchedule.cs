namespace HospitalSystem.Domain.Entities;

public class DefaultDoctorSchedule
{
    public Guid Id { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int AppointmentDurationMinutes { get; private set; }
    public bool IsActive { get; private set; }

    private DefaultDoctorSchedule() { }

    public static DefaultDoctorSchedule Create(
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int appointmentDurationMinutes)
    {
        return new DefaultDoctorSchedule
        {
            Id = Guid.NewGuid(),
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
