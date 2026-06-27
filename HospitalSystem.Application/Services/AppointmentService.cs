using HospitalSystem.Application.DTOs.Appointments;
using HospitalSystem.Application.Exceptions;
using HospitalSystem.Application.Interfaces;
using HospitalSystem.Application.Mapping;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Domain.Enums;
using HospitalSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HospitalSystem.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IUnitOfWork unitOfWork, ILogger<AppointmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AppointmentResponse> BookAppointmentAsync(
        CreateAppointmentRequest request,
        Guid receptionistId,
        CancellationToken cancellationToken = default)
    {
        ValidateNotInPast(request.AppointmentDate, request.StartTime);

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");

        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? throw new NotFoundException("Patient not found.");

        var schedule = await GetDoctorScheduleForDayAsync(request.DoctorId, request.AppointmentDate, cancellationToken);
        ValidateWithinWorkingHours(request.StartTime, schedule);

        var endTime = request.StartTime.Add(TimeSpan.FromMinutes(schedule.AppointmentDurationMinutes));

        if (endTime > schedule.EndTime)
            throw new ValidationException("Appointment exceeds doctor's working hours.");

        var isAvailable = await _unitOfWork.Appointments.IsSlotAvailableAsync(
            request.DoctorId, request.AppointmentDate, request.StartTime, endTime,
            cancellationToken: cancellationToken);

        if (!isAvailable)
            throw new ValidationException("Slot not available.");

        var appointment = Appointment.Create(
            request.PatientId,
            request.DoctorId,
            request.AppointmentDate,
            request.StartTime,
            endTime,
            receptionistId,
            request.Notes);

        await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Appointment booked {AppointmentId} for patient {PatientId} with doctor {DoctorId} on {Date} at {StartTime} by receptionist {ReceptionistId}",
            appointment.Id, request.PatientId, request.DoctorId, request.AppointmentDate, request.StartTime, receptionistId);

        return EntityMappers.ToResponse(appointment, patient.FullName, doctor.FullName);
    }

    public async Task<AppointmentResponse> RescheduleAsync(
        Guid appointmentId,
        RescheduleRequest request,
        Guid receptionistId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment not found.");

        if (appointment.Status is AppointmentStatus.Cancelled or AppointmentStatus.Completed)
            throw new ValidationException("Cannot reschedule a cancelled or completed appointment.");

        ValidateNotInPast(request.NewDate, request.NewStartTime);

        var schedule = await GetDoctorScheduleForDayAsync(appointment.DoctorId, request.NewDate, cancellationToken);
        ValidateWithinWorkingHours(request.NewStartTime, schedule);

        var newEndTime = request.NewStartTime.Add(TimeSpan.FromMinutes(schedule.AppointmentDurationMinutes));

        if (newEndTime > schedule.EndTime)
            throw new ValidationException("Appointment exceeds doctor's working hours.");

        var isAvailable = await _unitOfWork.Appointments.IsSlotAvailableAsync(
            appointment.DoctorId, request.NewDate, request.NewStartTime, newEndTime,
            appointmentId, cancellationToken);

        if (!isAvailable)
            throw new ValidationException("Slot not available.");

        appointment.Reschedule(request.NewDate, request.NewStartTime, newEndTime);
        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Appointment rescheduled {AppointmentId} to {Date} at {StartTime} by receptionist {ReceptionistId}",
            appointmentId, request.NewDate, request.NewStartTime, receptionistId);

        return await GetByIdAsync(appointmentId, cancellationToken);
    }

    public async Task CancelAsync(Guid appointmentId, Guid receptionistId, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment not found.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new ValidationException("Appointment is already cancelled.");

        appointment.Cancel();
        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Appointment cancelled {AppointmentId} by receptionist {ReceptionistId}",
            appointmentId, receptionistId);
    }

    public async Task<AppointmentResponse> CheckInAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment not found.");

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new ValidationException("Only confirmed appointments can be checked in.");

        appointment.CheckIn();
        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Appointment checked in {AppointmentId}", appointmentId);

        return await GetByIdAsync(appointmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableSlotResponse>> GetAvailableSlotsAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var schedule = await GetDoctorScheduleForDayAsync(doctorId, date, cancellationToken);
        var booked = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, date, cancellationToken);

        var slots = new List<AvailableSlotResponse>();
        var slotDuration = TimeSpan.FromMinutes(schedule.AppointmentDurationMinutes);
        var current = schedule.StartTime;

        while (current.Add(slotDuration) <= schedule.EndTime)
        {
            var end = current.Add(slotDuration);
            var overlaps = booked.Any(a =>
                a.Status != AppointmentStatus.Cancelled &&
                current < a.EndTime && end > a.StartTime);

            if (!overlaps)
                slots.Add(new AvailableSlotResponse(current, end));

            current = current.Add(slotDuration);
        }

        return slots;
    }

    public async Task<IReadOnlyList<AppointmentResponse>> GetDoctorScheduleAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var appointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, date, cancellationToken);
        var results = new List<AppointmentResponse>();

        foreach (var appointment in appointments)
        {
            results.Add(await MapAppointmentAsync(appointment, cancellationToken));
        }

        return results;
    }

    public async Task<AppointmentResponse> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment not found.");

        return await MapAppointmentAsync(appointment, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentResponse>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId, cancellationToken);
        var results = new List<AppointmentResponse>();

        foreach (var appointment in appointments)
        {
            results.Add(await MapAppointmentAsync(appointment, cancellationToken));
        }

        return results;
    }

    private async Task<DoctorSchedule> GetDoctorScheduleForDayAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var dayOfWeek = date.DayOfWeek;
        var allSchedules = await _unitOfWork.DoctorSchedules.GetAllAsync(cancellationToken);
        var schedule = allSchedules.FirstOrDefault(s =>
            s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek && s.IsActive);

        return schedule ?? throw new ValidationException("Doctor is not available on this day.");
    }

    private static void ValidateNotInPast(DateOnly date, TimeSpan startTime)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (date < today)
            throw new ValidationException("Appointment date cannot be in the past.");

        if (date == today)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            if (startTime <= now)
                throw new ValidationException("Appointment time cannot be in the past.");
        }
    }

    private static void ValidateWithinWorkingHours(TimeSpan startTime, DoctorSchedule schedule)
    {
        if (startTime < schedule.StartTime || startTime >= schedule.EndTime)
            throw new ValidationException("Start time is outside doctor's working hours.");
    }

    private async Task<AppointmentResponse> MapAppointmentAsync(
        Appointment appointment,
        CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(appointment.PatientId, cancellationToken)
            ?? throw new NotFoundException("Patient not found.");
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor not found.");
        return EntityMappers.ToResponse(appointment, patient.FullName, doctor.FullName);
    }
}
