namespace HospitalSystem.Domain.Entities;

public class DefaultDoctorDateSchedule
{
    public Guid Id { get; private set; }
    public DateOnly ScheduleDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int AppointmentDurationMinutes { get; private set; }
    public bool IsActive { get; private set; }

    private DefaultDoctorDateSchedule() { }

    public static DefaultDoctorDateSchedule Create(
        DateOnly scheduleDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int appointmentDurationMinutes)
    {
        return new DefaultDoctorDateSchedule
        {
            Id = Guid.NewGuid(),
            ScheduleDate = scheduleDate,
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
