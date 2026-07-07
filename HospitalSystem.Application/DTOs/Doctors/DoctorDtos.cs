using System.ComponentModel.DataAnnotations;

namespace HospitalSystem.Application.DTOs.Doctors;

public record CreateDoctorRequest(
[Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    string Password,

    [Required(ErrorMessage = "DepartmentId is required.")]
    Guid DepartmentId,

    [Required(ErrorMessage = "Full Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers or special characters.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    string FullName,

    [Required(ErrorMessage = "Specialization is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Specialization cannot contain numbers or special characters.")]
    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    string Specialization,

    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.")]
    string? Phone);
public record UpdateDoctorRequest(
       [Required(ErrorMessage = "DepartmentId is required.")]
    Guid DepartmentId,
        [Required(ErrorMessage = "Full Name is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers or special characters.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    string FullName,
         [Required(ErrorMessage = "Specialization is required.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "Specialization cannot contain numbers or special characters.")]
    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    string Specialization,

   [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.")]
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

public record SetDoctorDateScheduleRequest(
    DateOnly Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int AppointmentDurationMinutes);

public record DoctorDateScheduleResponse(
    Guid Id,
    DateOnly Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int AppointmentDurationMinutes,
    bool IsActive);

public record ApplyDefaultDateSchedulesRequest(IReadOnlyList<DateOnly> Dates);
