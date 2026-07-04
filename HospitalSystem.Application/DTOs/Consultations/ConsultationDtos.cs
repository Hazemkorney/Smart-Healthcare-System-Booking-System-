namespace HospitalSystem.Application.DTOs.Consultations;

public record StartConsultationRequest(Guid AppointmentId);

public record AddDiagnosisRequest(string Diagnosis, string? Notes);

public record CreatePrescriptionRequest(
    string MedicationName,
    string Dosage,
    string Frequency,
    string Duration,
    string? Notes);

public record PrescriptionResponse(
    Guid Id,
    string MedicationName,
    string Dosage,
    string Frequency,
    string Duration,
    string? Notes);

public record ConsultationResponse(
    Guid Id,
    Guid AppointmentId,
    Guid DoctorId,
    Guid PatientId,
    string? Diagnosis,
    string? Notes,
    DateTime StartedAt,
    DateTime? CompletedAt,
    IReadOnlyList<PrescriptionResponse> Prescriptions);

public record PatientMedicalHistoryEntry(
    Guid AppointmentId,
    DateOnly AppointmentDate,
    TimeSpan StartTime,
    string DoctorName,
    string? Diagnosis,
    string? Notes,
    IReadOnlyList<PrescriptionResponse> Prescriptions);
