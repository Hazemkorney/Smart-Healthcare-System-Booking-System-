using HospitalSystem.Application.DTOs.Consultations;
using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Application.DTOs.Appointments;

public record CreateAppointmentRequest(
    Guid PatientId,
    Guid DoctorId,
    DateOnly AppointmentDate,
    TimeSpan StartTime,
    string? Notes);

public record UpdateAppointmentRequest(string? Notes);

public record RescheduleRequest(DateOnly NewDate, TimeSpan NewStartTime);

public record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    DateOnly AppointmentDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    AppointmentStatus Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record AvailableSlotResponse(TimeSpan StartTime, TimeSpan EndTime);

public record AppointmentDetailResponse(
    AppointmentResponse Appointment,
    ConsultationResponse? Consultation);
