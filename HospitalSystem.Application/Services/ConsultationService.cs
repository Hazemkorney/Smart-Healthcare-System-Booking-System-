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
