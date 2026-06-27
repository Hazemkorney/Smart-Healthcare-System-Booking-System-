namespace HospitalSystem.Application.DTOs.Doctors;

public record CreateDoctorRequest(
    string Email,
    string Password,
    Guid DepartmentId,
    string FullName,
    string Specialization,
    string? Phone);

public record UpdateDoctorRequest(
    Guid DepartmentId,
    string FullName,
    string Specialization,
    string? Phone);

public record DoctorResponse(
    Guid Id,
    Guid UserId,
    Guid DepartmentId,
    string DepartmentName,
    string FullName,
    string Specialization,
    string? Phone,
    bool IsActive);

public record DoctorScheduleRequest(
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int AppointmentDurationMinutes);

public record SetDoctorScheduleRequest(IReadOnlyList<DoctorScheduleRequest> Schedules);

public record DoctorScheduleResponse(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int AppointmentDurationMinutes,
    bool IsActive);
