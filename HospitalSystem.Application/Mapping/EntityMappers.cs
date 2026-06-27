using HospitalSystem.Application.DTOs.Appointments;
using HospitalSystem.Application.DTOs.Consultations;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;

namespace HospitalSystem.Application.Mapping;

public static class EntityMappers
{
    public static AppointmentResponse ToResponse(
        Appointment appointment,
        string patientName,
        string doctorName) =>
        new(
            appointment.Id,
            appointment.PatientId,
            patientName,
            appointment.DoctorId,
            doctorName,
            appointment.AppointmentDate,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Status,
            appointment.Notes,
            appointment.CreatedAt,
            appointment.UpdatedAt);

    public static ConsultationResponse ToResponse(
        Consultation consultation,
        IEnumerable<Prescription> prescriptions) =>
        new(
            consultation.Id,
            consultation.AppointmentId,
            consultation.DoctorId,
            consultation.PatientId,
            consultation.Diagnosis,
            consultation.Notes,
            consultation.StartedAt,
            consultation.CompletedAt,
            prescriptions.Select(p => new PrescriptionResponse(
                p.Id, p.MedicationName, p.Dosage, p.Frequency, p.Duration, p.Notes)).ToList());
}
