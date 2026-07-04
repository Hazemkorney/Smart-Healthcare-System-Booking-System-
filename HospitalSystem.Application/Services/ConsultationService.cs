using HospitalSystem.Application.DTOs.Consultations;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Application.Mapping;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using HospitalSystem.Domain.Interfaces;

namespace HospitalSystem.Application.Services;

public class ConsultationService : IConsultationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ConsultationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ConsultationResponse> StartAsync(
        Guid appointmentId,
        Guid doctorId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await GetDoctorAppointmentAsync(appointmentId, doctorId, cancellationToken);

        if (appointment.Status is not (AppointmentStatus.Confirmed or AppointmentStatus.CheckedIn))
            throw new ValidationException("Appointment must be confirmed or checked in to start consultation.");

        ValidateConsultationCanStart(appointment);

        var existing = await FindConsultationByAppointmentAsync(appointmentId, cancellationToken);
        if (existing is not null)
            return await LoadConsultationResponseAsync(existing, cancellationToken);

        appointment.StartConsultation();
        var consultation = Consultation.Create(appointmentId, doctorId, appointment.PatientId);

        await _unitOfWork.Consultations.AddAsync(consultation, cancellationToken);
        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadConsultationResponseAsync(consultation, cancellationToken);
    }

    public async Task<ConsultationResponse> AddDiagnosisAsync(
        Guid appointmentId,
        Guid doctorId,
        AddDiagnosisRequest request,
        CancellationToken cancellationToken = default)
    {
        await GetDoctorAppointmentAsync(appointmentId, doctorId, cancellationToken);
        var consultation = await GetConsultationOrThrowAsync(appointmentId, cancellationToken);

        consultation.AddDiagnosis(request.Diagnosis, request.Notes);
        await _unitOfWork.Consultations.UpdateAsync(consultation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadConsultationResponseAsync(consultation, cancellationToken);
    }

    public async Task<ConsultationResponse> AddPrescriptionAsync(
        Guid appointmentId,
        Guid doctorId,
        CreatePrescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        await GetDoctorAppointmentAsync(appointmentId, doctorId, cancellationToken);
        var consultation = await GetConsultationOrThrowAsync(appointmentId, cancellationToken);

        var prescription = Prescription.Create(
            consultation.Id,
            request.MedicationName,
            request.Dosage,
            request.Frequency,
            request.Duration,
            request.Notes);

        await _unitOfWork.Prescriptions.AddAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadConsultationResponseAsync(consultation, cancellationToken);
    }

    public async Task<ConsultationResponse> CompleteAsync(
        Guid appointmentId,
        Guid doctorId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await GetDoctorAppointmentAsync(appointmentId, doctorId, cancellationToken);
        var consultation = await GetConsultationOrThrowAsync(appointmentId, cancellationToken);

        await ValidateConsultationReadyToCompleteAsync(consultation, cancellationToken);

        consultation.Complete();
        appointment.Complete();

        await _unitOfWork.Consultations.UpdateAsync(consultation, cancellationToken);
        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadConsultationResponseAsync(consultation, cancellationToken);
    }

    public async Task<ConsultationResponse?> GetByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        var consultation = await FindConsultationByAppointmentAsync(appointmentId, cancellationToken);
        return consultation is null ? null : await LoadConsultationResponseAsync(consultation, cancellationToken);
    }

    public async Task<IReadOnlyList<PatientMedicalHistoryEntry>> GetPatientMedicalHistoryAsync(
        Guid doctorId,
        Guid patientId,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
            ?? throw new NotFoundException("Patient not found.");

        var patientAppointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId, cancellationToken);
        if (!patientAppointments.Any(a => a.DoctorId == doctorId))
            throw new ForbiddenException("You can only view medical history for your patients.");

        return await BuildMedicalHistoryAsync(patientId, excludeAppointmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<PatientMedicalHistoryEntry>> GetOwnMedicalHistoryAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
            ?? throw new NotFoundException("Patient not found.");

        return await BuildMedicalHistoryAsync(patientId, excludeAppointmentId: null, cancellationToken);
    }

    private async Task<IReadOnlyList<PatientMedicalHistoryEntry>> BuildMedicalHistoryAsync(
        Guid patientId,
        Guid? excludeAppointmentId,
        CancellationToken cancellationToken)
    {
        var patientAppointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId, cancellationToken);
        var consultations = await _unitOfWork.Consultations.GetAllAsync(cancellationToken);
        var prescriptions = await _unitOfWork.Prescriptions.GetAllAsync(cancellationToken);
        var doctors = await _unitOfWork.Doctors.GetAllAsync(cancellationToken);

        var history = new List<PatientMedicalHistoryEntry>();
        foreach (var appointment in patientAppointments)
        {
            if (appointment.Status != AppointmentStatus.Completed)
                continue;

            if (excludeAppointmentId.HasValue && appointment.Id == excludeAppointmentId.Value)
                continue;

            var consultation = consultations.FirstOrDefault(c => c.AppointmentId == appointment.Id);
            if (consultation is null || consultation.CompletedAt is null)
                continue;

            if (string.IsNullOrWhiteSpace(consultation.Diagnosis))
                continue;

            var doctor = doctors.FirstOrDefault(d => d.Id == appointment.DoctorId)
                ?? throw new NotFoundException("Doctor not found.");

            var consultationPrescriptions = prescriptions.Where(p => p.ConsultationId == consultation.Id).ToList();
            history.Add(new PatientMedicalHistoryEntry(
                appointment.Id,
                appointment.AppointmentDate,
                appointment.StartTime,
                doctor.FullName,
                consultation.Diagnosis,
                consultation.Notes,
                consultationPrescriptions.Select(p => new PrescriptionResponse(
                    p.Id, p.MedicationName, p.Dosage, p.Frequency, p.Duration, p.Notes)).ToList()));
        }

        return history
            .OrderByDescending(h => h.AppointmentDate)
            .ThenByDescending(h => h.StartTime)
            .ToList();
    }

    private async Task<Appointment> GetDoctorAppointmentAsync(
        Guid appointmentId,
        Guid doctorId,
        CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment not found.");

        if (appointment.DoctorId != doctorId)
            throw new ForbiddenException("You can only access your own appointments.");

        return appointment;
    }

    private static void ValidateConsultationCanStart(Appointment appointment)
    {
        var appointmentStart = appointment.AppointmentDate.ToDateTime(
            TimeOnly.FromTimeSpan(appointment.StartTime),
            DateTimeKind.Utc);

        if (DateTime.UtcNow < appointmentStart)
            throw new ValidationException("Consultation cannot start before the scheduled appointment time.");
    }

    private async Task ValidateConsultationReadyToCompleteAsync(
        Consultation consultation,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(consultation.Diagnosis))
            throw new ValidationException("Diagnosis is required before completing the appointment.");

        await Task.CompletedTask;
    }

    private async Task<Consultation> GetConsultationOrThrowAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        return await FindConsultationByAppointmentAsync(appointmentId, cancellationToken)
            ?? throw new ValidationException("Consultation has not been started.");
    }

    private async Task<Consultation?> FindConsultationByAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var consultations = await _unitOfWork.Consultations.GetAllAsync(cancellationToken);
        return consultations.FirstOrDefault(c => c.AppointmentId == appointmentId);
    }

    private async Task<ConsultationResponse> LoadConsultationResponseAsync(
        Consultation consultation,
        CancellationToken cancellationToken)
    {
        var prescriptions = await _unitOfWork.Prescriptions.GetAllAsync(cancellationToken);
        var consultationPrescriptions = prescriptions.Where(p => p.ConsultationId == consultation.Id).ToList();
        return EntityMappers.ToResponse(consultation, consultationPrescriptions);
    }
}
